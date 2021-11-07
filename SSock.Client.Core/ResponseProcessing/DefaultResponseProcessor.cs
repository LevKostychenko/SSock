using SSock.Client.Core.Abstract.ResponseProcessing;
using SSock.Core.Services.Abstract.Communication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSock.Client.Core.ResponseProcessing
{
    internal class DefaultResponseProcessor<TResponse>
        : IResponseProcessor
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultResponseProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Process(IEnumerable<byte> payload)
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
        }
    }
}
