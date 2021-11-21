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

        public async Task<object> ExecuteAsync(
            byte[] tail,
            byte[] args,
            string clientId)
        {
            return await Task.Run(() =>
            {
                var hashPartLength = BitConverter.ToInt16(tail.Take(2).ToArray());
                var uploadingHash = args.Take(hashPartLength).BytesToString();

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

                    return 1;
                }

                return 1;
            });
        }
    }
}
