using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SSock.Core.Services.Abstract.FileUploading
{
    public interface IFileUploaderService
    {
        Task<string> InitFileUploadingSessionAsync(
            string currentSessionId,
            string saveLocation,
            string fileName);

        Task AppendFileAsync(
            string uploadingSessionId,
            string currentSessionId,
            IEnumerable<byte> data);

        Task<byte[]> GetFileBytesAsync(FileStream stream);

        Task<string> SaveFileAsync(
            string saveLocation,
            string fileName,
            byte[] fileData);

        bool IsUploadingSessionNotCommitted(string currentSessionId);

        void CommitUploadingSession(
            string currentSessionId,
            string uploadingSessionId);
    }
}
