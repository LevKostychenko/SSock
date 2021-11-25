using SSock.Client.Core.Abstract.ResponseProcessing;
using SSock.Core.Commands;
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
            Socket socket)
        {
            switch (command.ToUpper())
            {
                case CommandsNames.INIT_UPLOAD_COMMAND:
                    {
                        return new InitUploadResponseProcessor(
                            _serviceProvider,
                            socket);
                    }
                default:
                    {
                        return new DefaultResponseProcessor<string>(
                            _serviceProvider,
                            socket);
                    }
            }
        }
    }
}
