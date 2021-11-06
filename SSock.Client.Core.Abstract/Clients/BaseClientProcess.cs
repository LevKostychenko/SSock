﻿using Microsoft.Extensions.Configuration;
using SSock.Client.Domain;
using SSock.Core.Abstract;
using SSock.Core.Commands;
using SSock.Core.Infrastructure;
using SSock.Core.Services.Abstract.Communication;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SSock.Client.Core.Abstract.Clients
{
    // TODO: Rename to the Process
    public abstract class BaseClientProcess
        : BaseProcess<ClientPacket>, 
        IClient
    {
        protected virtual bool IsRunning { get; set; }

        private string ClientId { get; set; }

        private readonly IConfigurationSection _configurationSection;
        private readonly IPacketService<ServerPacket, ClientPacket> _packetService;

        protected BaseClientProcess(
            IConfigurationSection configurationSection,
            IPacketService<ServerPacket, ClientPacket> packetService)
        {
            _configurationSection = configurationSection;
            _packetService = packetService;
        }

        protected abstract Task ProcessUserCommandWithResponseAsync(
            string clientId,
            string command,
            ClientPacket receivedData,
            Socket socket);

        public virtual async Task RunAsync()
        {
            IsRunning = true;
            Socket socket = null;

            try
            {                
                var ipPoint = InitSocket(ref socket);
                await ConnectAsync(socket, ipPoint);

                while (IsRunning)
                {
                    var userCommand = Console.ReadLine();

                    await SendDataAsync(
                        socket,
                        _packetService.CreatePacket(
                            new ServerPacket
                            {
                                Command = userCommand,
                                ClientId = ClientId,
                                Payload = Encoding.Unicode.GetBytes(
                                    _packetService.GetCommandArgumnets(userCommand))
                            }));

                    var receivedData = await ReadDataAsync(socket);

                    await ProcessUserCommandWithResponseAsync(
                        ClientId,
                        userCommand, 
                        receivedData, 
                        socket);
                    Console.WriteLine($"{DateTime.Now} Response: " + receivedData);
                }
            }
            catch(Exception ex)
            {
               LogError($"Error: {ex.Message}");
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

        public virtual void Stop()
        {
            IsRunning = false;
        }

        private async Task ConnectAsync(
            Socket socket, 
            IPEndPoint ipPoint)
        {
            Console.WriteLine("Connection to the server...");
            socket.Connect(ipPoint);
            ClientId = Guid.NewGuid().ToString();

            await SendDataAsync(
                       socket,
                       _packetService.CreatePacket(
                           new ServerPacket
                           {
                               Command = CommandsNames.InitCommand,
                               ClientId = ClientId
                           }));

            var receivedData = await ReadDataAsync(socket);
            if (receivedData.Status == Statuses.Connected)
            {
                Console.WriteLine("Connected successfully");
            }
        }

        private IPEndPoint InitSocket(ref Socket socket)
        {
            var (port, address) = (
                    _configurationSection["port"],
                    _configurationSection["address"]);

            var ipPoint = new IPEndPoint(
                IPAddress.Parse(address),
                Int32.Parse(port));
            socket = new Socket(
                AddressFamily.InterNetwork, 
                SocketType.Stream, ProtocolType.Tcp);

            return ipPoint;
        }       
    }
}