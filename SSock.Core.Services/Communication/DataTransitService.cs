using SSock.Core.Services.Abstract.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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
    }
}
