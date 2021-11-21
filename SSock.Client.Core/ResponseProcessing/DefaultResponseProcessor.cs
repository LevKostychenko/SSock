using SSock.Client.Core.Abstract.ResponseProcessing;
using SSock.Core.Services.Abstract.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SSock.Client.Core.ResponseProcessing
{
    internal class DefaultResponseProcessor<TResponse>
        : IResponseProcessor
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly Socket _socket;

        public DefaultResponseProcessor(
            IServiceProvider serviceProvider,
            Socket socket)
        {
            _socket = socket;

            _serviceProvider = serviceProvider;
        }

        public async Task<object> ProcessAsync(
            IEnumerable<string> arguments,
            IEnumerable<byte> payload,
            string clientId)
        {
            return await Task.Run(() =>
            {
                var dataTransitService = (IDataTransitService)_serviceProvider
                    .GetService(typeof(IDataTransitService));

                var responseData = dataTransitService
                    .ConvertFromByteArray<TResponse>(
                        payload,
                        payload.Count());

                if (responseData != null
                    && responseData is string)
                {
                    Console.WriteLine("Response: " + responseData);
                }

                return responseData as object;
            });
        }
    }
}
