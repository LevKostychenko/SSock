using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Infrastructure.Extensions;
using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.FileUploading;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal class CommitUploadCommand
        : ICommand
    {
        private readonly IFileUploaderService _fileUploaderService;

        private const string Commited = "COMMITED";
        private const string AlreadyCommited = "ALREADY_COMMITED";

        public CommitUploadCommand(
            IFileUploaderService fileUploaderService
            )
        {
            _fileUploaderService = fileUploaderService;
        }

        public async Task<string> ExecuteAsync(
            byte[] args,
            string clientId)
        {
            return await Task.Run(() =>
            {
                var uploadingHash = args.BytesToString();

                if (string.IsNullOrEmpty(uploadingHash))
                {
                    throw new Exception("Some of arguments does not presents in the request");
                }

                var serverSessionId = ServerSession.SessionsIds
                        .Where(s => s.clientId == clientId)
                        .FirstOrDefault()
                        .sessionId;

                if (_fileUploaderService
                    .IsUploadingSessionNotCommitted(serverSessionId))
                {
                    _fileUploaderService.CommitUploadingSession(
                        serverSessionId,
                        uploadingHash);

                    return Commited;
                }

                return AlreadyCommited;
            });
        }
    }
}
