using System.IO;
using System.Threading.Tasks;

namespace SSock.Core.Services.Abstract.FileUploading
{
    public interface IFileUploaderService
    {
        Task<string> InitFileUploadingSessionAsync(string currentSessionId);

        Task<byte[]> GetFileBytesAsync(FileStream stream);

        Task<string> SaveFileAsync(
            string saveLocation,
            string fileName,
            byte[] fileData);

        bool IsUploadingSessionNotCommitted(string currentSessionId);

        void CommitUploadingSession(string currentSessionId);
    }
}
