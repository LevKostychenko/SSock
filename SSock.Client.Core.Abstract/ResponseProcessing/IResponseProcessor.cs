using System.Collections.Generic;

namespace SSock.Client.Core.Abstract.ResponseProcessing
{
    public interface IResponseProcessor
    {
        void Process(IEnumerable<byte> responsePayload);
    }
}
