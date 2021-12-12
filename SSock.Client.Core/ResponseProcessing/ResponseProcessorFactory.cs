using SSock.Client.Core.Abstract.ResponseProcessing;
using SSock.Core.Commands;
using SSock.Core.Infrastructure;
using System;
using System.Net.Sockets;

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
            string command,
            Ref<UdpClient> receiver,
            Ref<UdpClient> sender)
        {
            switch (command.ToUpper())
            {
                case CommandsNames.INIT_UPLOAD_COMMAND:
                    {
                        return new InitUploadResponseProcessor(
                            _serviceProvider,
                            sender,
                            receiver);
                    }
                default:
                    {
                        return new DefaultResponseProcessor<string>(
                            _serviceProvider,
                            sender,
                            receiver);
                    }
            }
        }
    }
}
