using Microsoft.Extensions.Configuration;
using SSock.Server.Core.Abstract.ServerEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SSock.Server.Core.ServerEngine
{
    internal class ServerRunner
        : IServerRunner
    {
        private readonly IConfiguration _configuration;

        private readonly IServerProcess _serverProcess;

        private bool IsRunning = true;

        public ServerRunner(
            IConfiguration configuration,
            IServerProcess serverProcess)
        {
            _configuration = configuration;
            _serverProcess = serverProcess;
        }

        public async Task RunAsync()
        {
            var section = _configuration.GetSection("listener");
            var (address, port) = (section["address"], section["port"]);
            var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(port));

            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(20);
               
                Console.WriteLine("Waiting for connections...");

                while (true)
                {
                    var socket = listenSocket.Accept();
                    await _serverProcess.ProcessAsync(socket, StopServer);
                }

                //while (IsRunning)
                //{
                //    var socket = listenSocket.Accept();
                //    await _serverProcess.ProcessAsync(socket, StopServer);

                //    Task.Run(async () => 
                //        await _serverProcess.ProcessAsync(socket, StopServer));
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (listenSocket != null)
                {                    
                    listenSocket.Close();
                }
            }
        }

        private void StopServer()
        {
            IsRunning = false;
        }
    }
}
