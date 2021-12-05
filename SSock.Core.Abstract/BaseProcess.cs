using SSock.Core.Services.Abstract.Communication;
using System;
using System.Collections.Generic;
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

        protected async Task SendDataAsync(UdpClient client, IEnumerable<byte> data)
            => await _dataTransitService.SendDataAsync(client, data);

        protected async Task<T> ReadDataAsync(UdpClient client)
            => await _dataTransitService
                    .ReadDataAsync(
                        client, 
                        READ_CHUNK_SIZE, 
                        x => ParsePacket(x));                    
    }
}
