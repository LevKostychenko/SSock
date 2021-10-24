using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.FileUploading;
using System;
using System.IO;
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

        public bool IsUploadingSessionNotCommitted(string currentSessionId)
        {
             var sessionKey = ServerSession
                    .SessionsCache[currentSessionId]
                    .Get<string>(UPLOADING_SESSION_KEY);

            return sessionKey != default;
        }
        
        public void CommitUploadingSession(string currentSessionId)
        {
            ServerSession
                .SessionsCache[currentSessionId]
                .Remove(UPLOADING_SESSION_KEY);
        }

        public async Task<string> SaveFileAsync(
            string saveLocation,
            string fileName,
            byte[] fileData)
        {
            var path = string.Empty;

            if (!string.IsNullOrEmpty(saveLocation)
                && !string.IsNullOrEmpty(fileName)
                && fileData != null)
            {
                path = $@"{saveLocation}\{fileName}";
                await File.WriteAllBytesAsync(path, fileData);
            }

            return path;
        }

        public async Task<byte[]> GetFileBytesAsync(FileStream stream)
        {
            var result = new byte[stream.Length];
            await stream.ReadAsync(result, 0, (int)stream.Length);

            return result;
        }
    }
}
