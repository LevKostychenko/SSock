using SSock.Core.Infrastructure;
using SSock.Core.Services.Abstract.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            UdpClient client,
            Func<IEnumerable<byte>, T> parsePacket,
            Ref<IPEndPoint> remoteEndPoint)
        {
            var result = await client.ReceiveAsync();

            remoteEndPoint.Value = result.RemoteEndPoint;
            return parsePacket(result.Buffer);
        }

        public async Task SendDataAsync(
            UdpClient client, 
            IEnumerable<byte> data,
            IPEndPoint endPoint)
        {
            if (data != null && data.Any())
            {
                await client.SendAsync(
                    data.ToArray(),
                    data.Count(),
                    endPoint);
            }
        }

        public bool IsSocketConnected(Socket socket)
            => !(socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0);
    }
}
