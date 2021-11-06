using SSock.Core.Services.Abstract.Communication;
using System;

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
    }
}
