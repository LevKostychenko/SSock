using SSock.Core.Abstract;
using SSock.Core.Commands;
using SSock.Core.Infrastructure;
using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.Communication;
using SSock.Server.Core.Abstract.CommandProcessing;
using SSock.Server.Core.Abstract.ServerEngine;
using SSock.Server.Domain;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SSock.Server.Core.ServerEngine
{
    internal class ServerProcess
        : BaseProcess<ServerPacket>,
        IServerProcess
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly IPacketService<ClientPacket, ServerPacket> _packetService;
        private readonly IDataTransitService _dataTransitService;

        public ServerProcess(
            ICommandProcessor commandProcessor,
            IPacketService<ClientPacket, ServerPacket> packetService,
            IDataTransitService dataTransitService)
            : base(dataTransitService)
        {
            _packetService = packetService;
            _dataTransitService = dataTransitService;
            _commandProcessor = commandProcessor;
        }

        // TODO: Refactor this method
        public async Task ProcessAsync(
            Socket socket,
            Action stopServerDelegate)
        {
            try
            {
                while (true)
                {
                    var packet = await ReadDataAsync(socket);

                    var (isNewClient, clientId) = IsNewClientConnected(packet);

                    if (isNewClient && !string.IsNullOrEmpty(clientId))
                    {
                        await NewClientConnectedAsync(clientId, socket);
                        continue;
                    }

                    Console.WriteLine($"{packet.Command}");
                    object response = string.Empty;

                    try
                    {
                        response = await _commandProcessor.ProcessAsync(packet);

                        if (IsRequestToClose(response))
                        {
                            await SendDataAsync(socket, _packetService.CreatePacket(
                                new ClientPacket
                                {
                                    Status = Statuses.Disconnected
                                }));
                            stopServerDelegate();
                            break;
                        }
                    }
                    catch (NotSupportedException)
                    {
                        await SendDataAsync(socket, _packetService.CreatePacket(
                            new ClientPacket
                            {
                                Status = Statuses.Unsupported
                            }));
                        continue;
                    }

                    await SendDataAsync(socket, _packetService.CreatePacket(
                            new ClientPacket
                            {
                                Status = Statuses.Ok,
                                Payload = _dataTransitService.ConvertToByteArray(response)
                            }));
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
            finally
            {
                if (socket != null)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
            }
        }

        private bool IsRequestToClose(object serverResponse)
            => ((string)serverResponse).Equals("close", StringComparison.OrdinalIgnoreCase);

        private (bool, string) IsNewClientConnected(ServerPacket packet)
        {
            if (packet.Command.Equals(
                CommandsNames.InitCommand,
                StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrEmpty(packet.ClientId))
            {
                return (true, packet.ClientId);
            }

            return (false, string.Empty);
        }

        private async Task NewClientConnectedAsync(string clientId, Socket socket)
        {
            ServerSession.InitNewSession(clientId);
            Console.WriteLine($"Client with ID {clientId} is connected.");
            await SendDataAsync(socket, _packetService.CreatePacket(
                new ClientPacket
                {
                    Status = Statuses.Connected
                }));
        }

        protected override ServerPacket ParsePacket(IEnumerable<byte> packet)
            => _packetService.ParsePacket(packet);   
    }
}
