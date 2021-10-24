using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.FileUploading;
using System.Linq;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal sealed class UploadFileCommand
        : ICommand
    {
        private readonly IFileUploaderService _fileUploaderService;

        public UploadFileCommand(IFileUploaderService fileUploaderService)
        {
            _fileUploaderService = fileUploaderService;
        }

        public async Task<string> ExecuteAsync(
            string[] commandArgumants,
            string clientId)
            => await _fileUploaderService
                .InitFileUploadingSessionAsync(
                ServerSession.SessionsIds
                    .Where(s => s.clientId == clientId)
                    .FirstOrDefault()
                    .sessionId);
        
    }
}
