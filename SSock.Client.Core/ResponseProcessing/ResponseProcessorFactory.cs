using SSock.Client.Core.Abstract.ResponseProcessing;
using System;

namespace SSock.Client.Core.ResponseProcessing
{
    internal class ResponseProcessorFactory
        : IResponseProcessorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ResponseProcessorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IResponseProcessor CreateResponseProcessor(
            string command)
        {
            switch (command)
            {                
                default:
                    {
                        return new DefaultResponseProcessor<string>(
                            _serviceProvider);
                    }
            }
        }
    }
}
