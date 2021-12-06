using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SSock.Core.Abstract
{
    public abstract class BaseProcess<T>
    {
        protected const int READ_CHUNK_SIZE = 2;
        protected abstract T ParsePacket(IEnumerable<byte> packet);
        
        protected void LogError(string error)
            => Console.WriteLine("Error: " + error);
    }
}
