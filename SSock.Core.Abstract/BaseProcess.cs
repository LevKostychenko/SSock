using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SSock.Core.Abstract
{
    public abstract class BaseProcess
    {
        protected const int READ_CHUNK_SIZE = 256;

        protected void LogError(string error)
        {
            Console.WriteLine("Error: " + error);
        }

        protected async Task SendDataAsync(NetworkStream stream, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var encodedCommand = Encoding.Unicode.GetBytes(message);
                await stream.WriteAsync(encodedCommand, 0, encodedCommand.Length);
            }
        }

        protected async Task<string> ReadDataAsync(NetworkStream stream)
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
