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

        public ServerRunner(
            IConfiguration configuration,
            IServerProcess serverProcess)
        {
            _configuration = configuration;
            _serverProcess = serverProcess;
        }

        public void Run()
        {
            var section = _configuration.GetSection("listener");
            var (address, port) = (section["address"], section["port"]);
            var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipPoint = new IPEndPoint(IPAddress.Parse(address), Int32.Parse(port));

            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(20);
               
                Console.WriteLine("Waiting for connections...");

                while (true)
                {
                    var socket = listenSocket.Accept();

                    Task.Run(async () => 
                        await _serverProcess.ProcessAsync(socket));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (listenSocket != null)
                {
                    listenSocket.Shutdown(SocketShutdown.Both);
                    listenSocket.Close();
                }
            }
        }
    }
}
