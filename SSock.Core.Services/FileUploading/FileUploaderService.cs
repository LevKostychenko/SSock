using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.FileUploading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SSock.Core.Services.FileUploading
{   
    internal class FileUploaderService
        : IFileUploaderService
    {
        private const string UPLOADING_SESSION_KEY = "UPLOADING_SESSION";

        public async Task<string> InitFileUploadingSessionAsync(
            string currentSessionId,
            string saveLocation,
            string fileName)
        {
            var uploadingSessionId = Guid.NewGuid().ToString();

            await ServerSession
                    .SessionsCache[currentSessionId]
                    .GetOrCreateAsync(UPLOADING_SESSION_KEY, uploadingSessionId);

            var path = $@"{saveLocation}\{fileName}";
            using var fs = File.Create(path);

            await ServerSession
                    .SessionsCache[currentSessionId]
                    .GetOrCreateAsync(uploadingSessionId, path);

            return uploadingSessionId;
        }

        public bool IsUploadingSessionNotCommitted(string currentSessionId)
        {
            var sessionKey = ServerSession
                   .SessionsCache[currentSessionId]
                   .Get<string>(UPLOADING_SESSION_KEY);

            return sessionKey != default;
        }

        public long GetUploadedBytes(string currentSessionId)
        {
            var uploadingSessionId = ServerSession
                   .SessionsCache[currentSessionId]
                   .Get<string>(UPLOADING_SESSION_KEY);

            if (!string.IsNullOrEmpty(uploadingSessionId))
            {
                var filePath = ServerSession
                    .SessionsCache[currentSessionId]
                    .Get<string>(uploadingSessionId);

                if (!string.IsNullOrEmpty(filePath))
                {
                    return new FileInfo(filePath).Length;
                }            
            }

            return 0;
        }

        public void CommitUploadingSession(
            string currentSessionId,
            string uploadingSessionId)
        {
            ServerSession
                .SessionsCache[currentSessionId]
                .Remove(UPLOADING_SESSION_KEY);

            ServerSession
                .SessionsCache[currentSessionId]
                .Remove(uploadingSessionId);
        }

        public async Task<long> AppendFileAsync(
            string uploadingSessionId,
            string currentSessionId,
            IEnumerable<byte> data)
        {
            var filePath = ServerSession
                .SessionsCache[currentSessionId]
                .Get<string>(uploadingSessionId);

            if (!string.IsNullOrEmpty(filePath))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Append))
                {
                    await fileStream.WriteAsync(data.ToArray(), 0, data.Count());
                }

                return new FileInfo(filePath).Length;
            }

            return 0;
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