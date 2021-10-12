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
            TcpListener listener = null;

            try
            {
                listener = new TcpListener(IPAddress.Parse(address), Int32.Parse(port));
                listener.Start();
                Console.WriteLine("Waiting for connections...");

                while (true)
                {
                    var client = listener.AcceptTcpClient();

                    Task.Run(async () => 
                        await _serverProcess.ProcessAsync(client));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (listener != null)
                {
                    listener.Stop();
                }
            }
        }
    }
}
