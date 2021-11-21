using System.Collections.Generic;

namespace SSock.Client.Services.Abstract
{
    public interface IUploadingService
    {
        IEnumerable<byte> GetUploadDataPayload(
            IEnumerable<byte> data, 
            string hash);
    }
}
