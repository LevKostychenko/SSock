using Microsoft.Extensions.Configuration;
using SSock.Core.Abstract;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SSock.Client.Core.Abstract.Clients
{
    public abstract class BaseClient
        : BaseProcess, 
        IClient
    {
        protected virtual bool IsRunning { get; set; }

        private readonly IConfigurationSection _configurationSection;

        protected BaseClient(IConfigurationSection configurationSection)
        {
            _configurationSection = configurationSection;
        }

        public virtual async Task RunAsync()
        {
            IsRunning = true;
            Socket socket = null;

            try
            {
                var (port, address) = (
                    _configurationSection["port"],
                    _configurationSection["address"]);

                var ipPoint = new IPEndPoint(IPAddress.Parse(address), Int32.Parse(port));
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);

                while (IsRunning)
                {
                    var userCommand = Console.ReadLine();
                    await SendDataAsync(socket, userCommand);

                    var receivedData = await ReadDataAsync(socket);
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
    }
}
