using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Infrastructure.Extensions;
using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.FileUploading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal class UploadDataCommand
        : ICommand
    {
        private readonly IFileUploaderService _fileUploaderService;

        private const string AlreadyCommited = "ALREADY_COMMITED";
        private const string Uploaded = "ALREADY_COMMITED";

        public UploadDataCommand(IFileUploaderService fileUploaderService)
        {
            _fileUploaderService = fileUploaderService;
        }

        public async Task<string> ExecuteAsync(
            byte[] args,
            string clientId)
        {
            (string uploadingHash, IEnumerable<byte> data) =
                (
                    args.Take(64).BytesToString(), 
                    args.TakeLast(960)
                );

            if (string.IsNullOrEmpty(uploadingHash)
                || data == null
                || !data.Any())
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
                await _fileUploaderService.AppendFileAsync(
                    uploadingHash, 
                    serverSessionId, 
                    data);

                return Uploaded;
            }

            return AlreadyCommited;
        }
    }
}
