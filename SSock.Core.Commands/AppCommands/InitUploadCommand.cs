using Microsoft.Extensions.Configuration;
using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Infrastructure.Extensions;
using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.FileUploading;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal sealed class InitUploadCommand
        : ICommand
    {
        private readonly IFileUploaderService _fileUploaderService;

        private readonly IConfiguration _configuration;

        public InitUploadCommand(
            IFileUploaderService fileUploaderService,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _fileUploaderService = fileUploaderService;
        }

        public async Task<object> ExecuteAsync(
            byte[] tail,
            byte[] args,
            string clientId)
            => await _fileUploaderService
                .InitFileUploadingSessionAsync(                   
                    ServerSession.SessionsIds
                        .Where(s => s.clientId == clientId)
                        .FirstOrDefault()
                        .sessionId,
                     _configuration.GetSection("filestorage")["savelocation"],
                     string.IsNullOrEmpty(args.BytesToString()) 
                        ? Guid.NewGuid().ToString() 
                        : args.BytesToString());        
    }
}
