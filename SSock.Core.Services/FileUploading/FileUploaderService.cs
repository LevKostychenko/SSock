using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.FileUploading;
using System;
using System.Threading.Tasks;

namespace SSock.Core.Services.FileUploading
{
    internal class FileUploaderService
        : IFileUploaderService
    {
        private const string UPLOADING_SESSION_KEY = "UPLOADING_SESSION";

        public async Task<string> InitFileUploadingSessionAsync(string currentSessionId)
        {
            var uploadingSessionId = Guid.NewGuid().ToString();

            await ServerSession
                    .SessionsCache[currentSessionId]
                    .GetOrCreateAsync(UPLOADING_SESSION_KEY, uploadingSessionId);

            return uploadingSessionId;
        }
    }
}
