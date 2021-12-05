using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SSock.Core.Services.Abstract.Communication
{
    public interface IDataTransitService
    {
        byte[] AppendBytes(byte[] initialBytes, int count);

        byte[] ConvertToByteArray<T>(T data);

        T ConvertFromByteArray<T>(
            IEnumerable<byte> data,
            int actualDataLength);

        Task<T> ReadDataAsync<T>(
           UdpClient client,
           int chunkSize,
           Func<IEnumerable<byte>, T> parsePacket);

        Task SendDataAsync(
            UdpClient client,
            IEnumerable<byte> data);

        bool IsSocketConnected(Socket socket);
    }
}
