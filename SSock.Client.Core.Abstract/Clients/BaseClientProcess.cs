using Microsoft.Extensions.Configuration;
using SSock.Client.Domain;
using SSock.Core.Abstract;
using SSock.Core.Commands;
using SSock.Core.Infrastructure;
using SSock.Core.Services.Abstract.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SSock.Client.Core.Abstract.Clients
{
    public abstract class BaseClientProcess
        : BaseProcess<ClientPacket>, 
        IClient
    {
        protected virtual bool IsRunning { get; set; }

        private string ClientId { get; set; }

        private readonly IConfigurationSection _configurationSection;
        private readonly IPacketService<ServerPacket, ClientPacket> _packetService;
        private readonly IDataTransitService _dataTransitService;

        protected BaseClientProcess(
            IDataTransitService dataTransitService,
            IConfigurationSection configurationSection,
            IPacketService<ServerPacket, ClientPacket> packetService)
            : base(dataTransitService)
        {
            _configurationSection = configurationSection;
            _packetService = packetService;
            _dataTransitService = dataTransitService;
        }

        protected abstract Task ProcessUserCommandWithResponseAsync(
            string clientId,
            (string command, IEnumerable<string> arguments) command,
            ClientPacket receivedData,
            Ref<UdpClient> client);

        public virtual async Task RunAsync()
        {
            IsRunning = true;
            var client = new Ref<UdpClient>();

            try
            {                
                var ipPoint = InitClient(client);
                await ConnectAsync(client, ipPoint);

                while (IsRunning)
                {
                    var parsedCommand = _packetService.GetCommandParts(Console.ReadLine());

                    await SendDataAsync(
                        client,
                        _packetService.CreatePacket(
                            new ServerPacket
                            {
                                Command = parsedCommand.command,
                                ClientId = ClientId,
                                Payload = _dataTransitService.ConvertToByteArray(
                                    parsedCommand.arguments == null 
                                        ? new List<string> { string.Empty }
                                        : parsedCommand.arguments.ToList())
                            }));

                    var receivedData = await ReadDataAsync(client);

                    await ProcessUserCommandWithResponseAsync(
                        ClientId,
                        parsedCommand,
                        receivedData,
                        client);
                }
            }
            catch(Exception ex)
            {
               LogError($"Error: {ex.Message}");
            }
            finally
            {
                if (client != null)
                {
                    client.Value.Close();
                }
            }
        }

        public virtual void Stop()
        {
            IsRunning = false;
        }

        private async Task ConnectAsync(
            UdpClient client, 
            IPEndPoint ipPoint)
        {
            Console.WriteLine("Connection to the server...");
            client.Connect(ipPoint);
            ClientId = Guid.NewGuid().ToString();

            await SendDataAsync(
                       client,
                       _packetService.CreatePacket(
                           new ServerPacket
                           {
                               Command = CommandsNames.INIT_COMMAND,
                               ClientId = ClientId  
                           }));

            var receivedData = await ReadDataAsync(client);
            if (receivedData.Status == Statuses.Connected)
            {
                Console.WriteLine("Connected successfully");
            }
        }

        private IPEndPoint InitClient(Ref<UdpClient> client)
        {
            var (port, address) = (
                    _configurationSection["port"],
                    _configurationSection["address"]);

            var ipPoint = new IPEndPoint(
                IPAddress.Parse(address),
                Int32.Parse(port));
            client.Value = new UdpClient(ipPoint);

            return ipPoint;
        }
    }
}
