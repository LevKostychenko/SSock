using System.Threading.Tasks;

namespace SSock.Core.Services.Abstract.FileUploading
{
    public interface IFileUploaderService
    {
        Task<string> InitFileUploadingSessionAsync(string currentSessionId);
    }
}
