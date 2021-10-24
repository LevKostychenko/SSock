using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SSock.Core.Abstract
{
    public abstract class BaseProcess
    {
        protected const int READ_CHUNK_SIZE = 256;
        protected const string INIT_MESSAGE = "INIT";
        protected const string CONNECTED_MESSAGE = "CONNECTED";
        
        protected void LogError(string error)
        {
            Console.WriteLine("Error: " + error);
        }

        protected async Task SendDataAsync(Socket socket, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var encodedCommand = Encoding.Unicode.GetBytes(message);
                await socket.SendAsync(new ArraySegment<byte>(encodedCommand), SocketFlags.None);
            }
        }

        protected async Task<string> ReadDataAsync(Socket socket)
        {
            var data = new ArraySegment<byte>(new byte[READ_CHUNK_SIZE]);
            var builder = new StringBuilder();

            do
            {
                var bytes = await socket.ReceiveAsync(data, SocketFlags.None);
                builder.Append(Encoding.Unicode.GetString(data.Array, 0, bytes));
            }
            while (socket.Available > 0);

            return builder.ToString();
        }
    }
}
