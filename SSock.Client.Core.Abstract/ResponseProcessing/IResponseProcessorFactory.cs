﻿using System.Net.Sockets;

namespace SSock.Client.Core.Abstract.ResponseProcessing
{
    public interface IResponseProcessorFactory
    {
        IResponseProcessor CreateResponseProcessor(
            string command,
            Socket socket);
    }
}