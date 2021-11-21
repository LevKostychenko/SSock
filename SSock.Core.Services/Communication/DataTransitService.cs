using SSock.Core.Services.Abstract.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace SSock.Core.Services.Communication
{
    internal class DataTransitService
        : IDataTransitService
    {
        public byte[] AppendBytes(byte[] initialBytes, int count)
        {
            var prevLength = initialBytes.Length;

            Array.Resize<byte>(
                ref initialBytes,
                initialBytes.Length + (count - initialBytes.Length));

            for (int i = prevLength; i < initialBytes.Length; i++)
            {
                initialBytes[i] = 0;
            }

            return initialBytes;
        }

        public T ConvertFromByteArray<T>(
            IEnumerable<byte> data,
            int actualDataLength)
        {
            if (data == null 
                || !data.Any()
                || actualDataLength == 0)
            {
                return default;
            }

            if (actualDataLength > data.Count())
            {
                throw new ArgumentOutOfRangeException(nameof(actualDataLength));
            }

            var formatter = new BinaryFormatter();

            using (var stream = new MemoryStream(
                data
                    .Take(actualDataLength)
                    .ToArray()))
            {                
                return (T)formatter.Deserialize(stream);
            }
        }

        public byte[] ConvertToByteArray<T>(T data)
        {
            if (EqualityComparer<T>
                .Default
                .Equals(data, default(T)))
            {
                return null;
            }

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, data);
                return stream.ToArray();
            }
        }

        public async Task<T> ReadDataAsync<T>(
            Socket socket, 
            int chunkSize,
            Func<IEnumerable<byte>, T> parsePacket)
        {
            var data = new ArraySegment<byte>(new byte[chunkSize]);
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

            return parsePacket(receivedPacket);
        }

        public async Task SendDataAsync(
            Socket socket, 
            IEnumerable<byte> data)
        {
            if (data != null && data.Any())
            {
                await socket.SendAsync(
                    new ArraySegment<byte>(data.ToArray()),
                    SocketFlags.None);
            }
        }

        public async Task SendDataAsync(
            Socket socket,
            FileStream fileStream)
        {
            using (var networkStream = new BufferedStream(new NetworkStream(socket, false)))
            {
                await fileStream.CopyToAsync(networkStream);
                await networkStream.FlushAsync();
            }
        }
    }
}
