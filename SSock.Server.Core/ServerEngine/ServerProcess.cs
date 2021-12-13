using Microsoft.Extensions.Configuration;
using SSock.Core;
using SSock.Core.Commands;
using SSock.Core.Infrastructure;
using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.Communication;
using SSock.Server.Core.Abstract.CommandProcessing;
using SSock.Server.Core.Abstract.ServerEngine;
using SSock.Server.Domain;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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

        private Ref<IPEndPoint> _remoteEndPoint = new Ref<IPEndPoint>();

        public ServerProcess(
            ICommandProcessor commandProcessor,
            IPacketService<ClientPacket, ServerPacket> packetService,
            IDataTransitService dataTransitService,
            IConfiguration configuration)
            : base(
                  Int32.Parse(configuration.GetSection("listener")["localPort"]))
        {
            _packetService = packetService;
            _dataTransitService = dataTransitService;
            _commandProcessor = commandProcessor;
        }

        // TODO: Refactor this method
        public async Task ProcessAsync(
            Action stopServerDelegate)
        {
            try
            {
                while (true)
                {                    
                    var packet = await _dataTransitService.ReadDataAsync(
                        Client, 
                        x => ParsePacket(x),
                        _remoteEndPoint);                  
                    var (isNewClient, clientId) = IsNewClientConnected(packet);

                    if (isNewClient && !string.IsNullOrEmpty(clientId))
                    {
                        await NewClientConnectedAsync(clientId);
                        continue;
                    }

                    Console.WriteLine($"{packet.Command}");
                    object response = null;

                    try
                    {
                        response = await _commandProcessor.ProcessAsync(packet);

                        if (IsRequestToClose(response))
                        {
                            await _dataTransitService.SendDataAsync(
                                Client,
                                _packetService.CreatePacket(
                                new ClientPacket
                                {
                                    Status = Statuses.Disconnected
                                }),
                                _remoteEndPoint.Value);
                            stopServerDelegate();
                            break;
                        }
                    }
                    catch (NotSupportedException)
                    {
                        await _dataTransitService.SendDataAsync(
                            Client,
                            _packetService.CreatePacket(
                            new ClientPacket
                            {
                                Status = Statuses.Unsupported
                            }),
                            _remoteEndPoint.Value);
                        continue;
                    }

                    await _dataTransitService.SendDataAsync(
                        Client, 
                        _packetService.CreatePacket(
                            new ClientPacket
                            {
                                Status = Statuses.Ok,
                                Payload = _dataTransitService.ConvertToByteArray(response)
                            }),
                        _remoteEndPoint.Value);
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
            finally
            {
            }
        }

        private bool IsRequestToClose(object serverResponse)
            => serverResponse is string && ((string)serverResponse).Equals("close", StringComparison.OrdinalIgnoreCase);

        private (bool, string) IsNewClientConnected(ServerPacket packet)
        {
            if (packet.Command.Equals(
                CommandsNames.INIT_COMMAND,
                StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrEmpty(packet.ClientId))
            {
                return (true, packet.ClientId);
            }

            return (false, string.Empty);
        }

        private async Task NewClientConnectedAsync(string clientId)
        {
            ServerSession.InitNewSession(clientId);
            Console.WriteLine($"Client with ID {clientId} is connected.");
            await _dataTransitService.SendDataAsync(
                Client, 
                _packetService.CreatePacket(
                new ClientPacket
                {
                    Status = Statuses.Connected
                }),
                _remoteEndPoint.Value);
        }

        protected override ServerPacket ParsePacket(IEnumerable<byte> packet)
            => _packetService.ParsePacket(packet);   
    }
}
