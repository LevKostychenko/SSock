using System.Collections.Generic;

namespace SSock.Core.Services.Abstract.Communication
{
    public interface IDataTransitService
    {
        byte[] AppendBytes(byte[] initialBytes, int count);

        byte[] ConvertToByteArray<T>(T data);

        T ConvertFromByteArray<T>(
            IEnumerable<byte> data,
            int actualDataLength);
    }
}
