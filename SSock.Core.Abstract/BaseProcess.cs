using SSock.Core.Services.Abstract.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SSock.Core.Abstract
{
    public abstract class BaseProcess<T>
    {
        protected const int READ_CHUNK_SIZE = 2;
        protected abstract T ParsePacket(IEnumerable<byte> packet);

        private readonly IDataTransitService _dataTransitService;

        public BaseProcess(IDataTransitService dataTransitService)
        {
            _dataTransitService = dataTransitService;
        }

        protected void LogError(string error)
            => Console.WriteLine("Error: " + error);        

        protected async Task SendDataAsync(Socket socket, IEnumerable<byte> data)
            => await _dataTransitService.SendDataAsync(socket, data);

        protected async Task SendDataAsync(Socket socket, FileStream fileStream)
            => await _dataTransitService.SendDataAsync(socket, fileStream);

        protected async Task<T> ReadDataAsync(Socket socket)
            => await _dataTransitService
                    .ReadDataAsync(
                        socket, 
                        READ_CHUNK_SIZE, 
                        x => ParsePacket(x));                    
    }
}
