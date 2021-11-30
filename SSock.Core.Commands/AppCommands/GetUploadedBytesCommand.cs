using Microsoft.Extensions.Configuration;
using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.Communication;
using SSock.Core.Services.Abstract.FileUploading;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal class GetUploadedBytesCommand
        : ICommand
    {
        private readonly IFileUploaderService _fileUploaderService;
        private readonly IConfiguration _configuration;
        private readonly IDataTransitService _dataTransitService;

        public GetUploadedBytesCommand(
            IFileUploaderService fileUploaderService,
            IConfiguration configuration,
            IDataTransitService dataTransitService)
        {
            _configuration = configuration;
            _fileUploaderService = fileUploaderService;
            _dataTransitService = dataTransitService;
        }

        public async Task<object> ExecuteAsync(
            byte[] tail,
            byte[] args,
            string clientId)
            => await Task.Run(() =>
                    {
                        var currentServerSession = ServerSession.SessionsIds
                                   .Where(s => s.clientId == clientId)
                                   .FirstOrDefault()
                                   .sessionId;

                        return _fileUploaderService.GetUploadedBytes(currentServerSession);
                    });
    }
}
