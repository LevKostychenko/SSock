using Microsoft.Extensions.Configuration;
using SSock.Server.Core.Abstract.ServerEngine;
using System;
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
            var client = new UdpClient(Int32.Parse(port));

            try
            {               
                Console.WriteLine("Waiting for connections...");

                while (true)
                {
                    await _serverProcess.ProcessAsync(client, StopServer);
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
                if (client != null)
                {
                    client.Close();
                }
            }
        }

        private void StopServer()
        {
            IsRunning = false;
        }
    }
}
