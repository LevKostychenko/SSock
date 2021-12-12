using SSock.Client.Core.Abstract.ResponseProcessing;
using SSock.Core.Infrastructure;
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

        private readonly Ref<UdpClient> _sender;
        private readonly Ref<UdpClient> _receiver;

        public DefaultResponseProcessor(
            IServiceProvider serviceProvider,
            Ref<UdpClient> sender,
            Ref<UdpClient> receiver)
        {
            _receiver = receiver;
            _sender = sender;

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
