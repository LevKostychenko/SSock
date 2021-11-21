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

        public async Task<object> ExecuteAsync(
            byte[] tail,
            byte[] args,
            string clientId)
        {            
            var uploadingHashLength = BitConverter.ToInt16(tail.Skip(6).Take(2).ToArray());
            var uploadingChunkLength = BitConverter.ToInt16(tail.Skip(8).Take(2).ToArray());

            (string uploadingHash, IEnumerable<byte> data) =
                (
                    args.Take(uploadingHashLength).BytesToString(),
                    args.TakeLast(960).Take(uploadingChunkLength)
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
                var nextOffset = await _fileUploaderService.AppendFileAsync(
                    uploadingHash, 
                    serverSessionId, 
                    data);

                return nextOffset;
            }

            return 0;
        }
    }
}
