using Microsoft.Extensions.Configuration;
using SSock.Client.Domain;
using SSock.Core;
using SSock.Core.Commands;
using SSock.Core.Infrastructure;
using SSock.Core.Services.Abstract.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SSock.Client.Core.Abstract.Clients
{
    public abstract class BaseClientProcess
        : BaseProcess<ClientPacket>,
        IClient
    {
        protected virtual bool IsRunning { get; set; }

        private string ClientId { get; set; }

        private readonly IConfiguration _configuration;
        private readonly IPacketService<ServerPacket, ClientPacket> _packetService;
        private readonly IDataTransitService _dataTransitService;

        private Ref<IPEndPoint> remoteEndPoint = new Ref<IPEndPoint>();

        protected BaseClientProcess(
            IDataTransitService dataTransitService,
            IConfiguration configuration,
            IPacketService<ServerPacket, ClientPacket> packetService)
            : base(
                  Int32.Parse(configuration["localSenderPort"]),
                  Int32.Parse(configuration["localReceiverPort"]))
        {
            _configuration = configuration;
            _packetService = packetService;
            _dataTransitService = dataTransitService;
        }

        protected abstract Task ProcessUserCommandWithResponseAsync(
            string clientId,
            (string command, IEnumerable<string> arguments) command,
            ClientPacket receivedData);

        public virtual async Task RunAsync()
        {
            IsRunning = true;

            try
            {
                var ipPoint = InitClient();
                await ConnectAsync(ipPoint);

                while (IsRunning)
                {
                    var parsedCommand = _packetService.GetCommandParts(Console.ReadLine());

                    await _dataTransitService.SendDataAsync(
                        Sender,
                        _packetService.CreatePacket(
                            new ServerPacket
                            {
                                Command = parsedCommand.command,
                                ClientId = ClientId,
                                Payload = _dataTransitService.ConvertToByteArray(
                                    parsedCommand.arguments == null
                                        ? new List<string> { string.Empty }
                                        : parsedCommand.arguments.ToList())
                            }),
                        ipPoint);

                    var receivedData = await _dataTransitService.ReadDataAsync(
                        Receiver,
                        x => _packetService.ParsePacket(x),
                        remoteEndPoint);

                    await ProcessUserCommandWithResponseAsync(
                        ClientId,
                        parsedCommand,
                        receivedData);
                }
            }
            catch (Exception ex)
            {
                LogError($"Error: {ex.Message}");
            }
            finally
            {                
            }
        }

        public virtual void Stop()
        {
            IsRunning = false;
        }

        private async Task ConnectAsync(
            IPEndPoint ipPoint)
        {
            Console.WriteLine("Connection to the server...");
            // client.Connect(ipPoint);
            ClientId = Guid.NewGuid().ToString();

            await _dataTransitService.SendDataAsync(
                       Sender,
                       _packetService.CreatePacket(
                           new ServerPacket
                           {
                               Command = CommandsNames.INIT_COMMAND,
                               ClientId = ClientId
                           }),
                       ipPoint);

            var receivedData = await _dataTransitService.ReadDataAsync(
                Receiver,
                x => _packetService.ParsePacket(x),
                remoteEndPoint);
            if (receivedData.Status == Statuses.Connected)
            {
                Console.WriteLine("Connected successfully");
            }
        }

        private IPEndPoint InitClient()
        {
            var (port, address) = (
                    _configuration.GetSection("server")["port"],
                    _configuration.GetSection("server")["address"]);

            var ipPoint = new IPEndPoint(
                IPAddress.Parse(address),
                Int32.Parse(port));            

            return ipPoint;
        }
    }
}
