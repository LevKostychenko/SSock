using System;
using System.Collections.Generic;
using System.IO;
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
           Socket socket,
           int chunkSize,
           Func<IEnumerable<byte>, T> parsePacket);

        Task SendDataAsync(
            Socket socket,
            IEnumerable<byte> data);

        Task SendDataAsync(
            Socket socket,
            FileStream fileStream);
    }
}
