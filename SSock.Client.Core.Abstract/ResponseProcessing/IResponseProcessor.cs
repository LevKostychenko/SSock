using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSock.Client.Core.Abstract.ResponseProcessing
{
    public interface IResponseProcessor
    {
        Task<object> ProcessAsync(
            IEnumerable<string> arguments,
            IEnumerable<byte> responsePayload,
            string clientId);
    }
}
