using Microsoft.Extensions.Configuration;
using SSock.Core.Abstract;
using System;
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
            TcpClient client = null;

            try
            {
                var (port, address) = (
                    _configurationSection["port"],
                    _configurationSection["address"]);
                client = new TcpClient(address, Int32.Parse(port));
                var stream = client.GetStream();

                while (IsRunning)
                {
                    var userCommand = Console.ReadLine();
                    await SendDataAsync(stream, userCommand);

                    var receivedData = await ReadDataAsync(stream);
                }
            }
            catch(Exception ex)
            {
               LogError($"Error: {ex.Message}");
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
    }
}
