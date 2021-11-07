using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SSock.Core.Abstract
{
    public abstract class BaseProcess<T>
    {
        protected const int READ_CHUNK_SIZE = 2;
        protected abstract T ParsePacket(IEnumerable<byte> packet);

        protected void LogError(string error)
        {
            Console.WriteLine("Error: " + error);
        }

        protected async Task SendDataAsync(Socket socket, IEnumerable<byte> data)
        {
            if (data != null && data.Any())
            {
                await socket.SendAsync(
                    new ArraySegment<byte>(data.ToArray()), 
                    SocketFlags.None);
            }
        }

        protected async Task SendDataAsync(Socket socket, FileStream fileStream)
        {
            using (var networkStream = new BufferedStream(new NetworkStream(socket, false)))
            {
                await fileStream.CopyToAsync(networkStream);
                await networkStream.FlushAsync();
            }
        }

        protected async Task<T> ReadDataAsync(Socket socket)
        {
            var data = new ArraySegment<byte>(new byte[READ_CHUNK_SIZE]);
            var receivedPacket = new List<byte>();

            do
            {
                var bytes = await socket.ReceiveAsync(data, SocketFlags.None);

                if (data.Array != null)
                {
                    receivedPacket.AddRange(data.Array);
                }
            }
            while (socket.Available > 0);

            return ParsePacket(receivedPacket);
        }
    }
}
