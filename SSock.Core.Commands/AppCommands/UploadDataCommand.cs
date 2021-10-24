using Microsoft.Extensions.Configuration;
using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.FileUploading;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal class UploadDataCommand
        : ICommand
    {
        private readonly IFileUploaderService _fileUploaderService;

        private readonly IConfiguration _configuration;

        public UploadDataCommand(
            IFileUploaderService fileUploaderService,
            IConfiguration configuration
            )
        {
            _configuration = configuration;
            _fileUploaderService = fileUploaderService;
        }

        public async Task<string> ExecuteAsync(
            string[] commandArgumants,
            string clientId)
        {
            (string uploadingHash, string fileName, string data) =
                (commandArgumants[0], commandArgumants[1], commandArgumants[2]);

            if (string.IsNullOrEmpty(uploadingHash)
                || string.IsNullOrEmpty(data)
                || string.IsNullOrEmpty(fileName))
            {
                throw new Exception("Some of arguments does not presents in the request");
            }

            var filePath = string.Empty;

            var serverSessionId = ServerSession.SessionsIds
                    .Where(s => s.clientId == clientId)
                    .FirstOrDefault()
                    .sessionId;

            if (_fileUploaderService
                .IsUploadingSessionNotCommitted(serverSessionId))
            {
                filePath = await _fileUploaderService.SaveFileAsync(
                     _configuration.GetSection("filestorage")["savelocation"],
                     fileName,
                     Encoding.Unicode.GetBytes(data));

                _fileUploaderService.CommitUploadingSession(serverSessionId);
            }

            return filePath;
        }
    }
}
