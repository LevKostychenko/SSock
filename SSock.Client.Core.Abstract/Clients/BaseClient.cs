using Microsoft.Extensions.Configuration;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SSock.Client.Core.Abstract.Clients
{
    public abstract class BaseClient
        : IClient
    {
        const int READ_CHUNK_SIZE = 256;

        protected virtual bool IsRunning { get; set; }

        private readonly IConfigurationSection _configurationSection;

        protected BaseClient(IConfigurationSection configurationSection)
        {
            _configurationSection = configurationSection;
        }

        public virtual async Task RunAsync()
        {
            IsRunning = true;
            TcpClient client = null;

            try
            {
                var (port, address) = (_configurationSection["port"], _configurationSection["address"]);
                client = new TcpClient(address, Int32.Parse(port));
                var stream = client.GetStream();

                while (IsRunning)
                {
                    await SendDataAsync(stream);

                    var receivedData = await ReadDataAsync(stream);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }

        public virtual void Stop()
        {
            IsRunning = false;
        }

        private async Task SendDataAsync(NetworkStream stream)
        {
            var userCommand = Console.ReadLine();

            if (!string.IsNullOrEmpty(userCommand))
            {
                var encodedCommand = Encoding.Unicode.GetBytes(userCommand);
                await stream.WriteAsync(encodedCommand, 0, encodedCommand.Length);
            }
        }

        private async Task<string> ReadDataAsync(NetworkStream stream)
        {
            var data = new byte[READ_CHUNK_SIZE];
            var builder = new StringBuilder();

            do
            {
                var bytes = await stream.ReadAsync(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            return builder.ToString();
        }
    }
}
