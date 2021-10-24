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

        private string ClientId { get; set; }

        private readonly IConfigurationSection _configurationSection;

        protected BaseClient(IConfigurationSection configurationSection)
        {
            _configurationSection = configurationSection;
        }

        protected abstract Task ProcessUserCommandWithResponseAsync(
            string command,
            string receivedData,
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

                    await SendDataAsync(socket, userCommand.Trim() + $" {ClientId}");
                    var receivedData = await ReadDataAsync(socket);

                    await ProcessUserCommandWithResponseAsync(
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

            await SendDataAsync(socket, $"{INIT_MESSAGE} {ClientId}");

            var receivedData = await ReadDataAsync(socket);
            if (receivedData == CONNECTED_MESSAGE)
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
