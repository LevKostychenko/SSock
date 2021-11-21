using Microsoft.Extensions.Configuration;
using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Infrastructure.Extensions;
using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.Communication;
using SSock.Core.Services.Abstract.FileUploading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal sealed class InitUploadCommand
        : ICommand
    {
        private readonly IFileUploaderService _fileUploaderService;
        private readonly IConfiguration _configuration;
        private readonly IDataTransitService _dataTransitService;

        public InitUploadCommand(
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
        {
            var currentServerSession = ServerSession.SessionsIds
                        .Where(s => s.clientId == clientId)
                        .FirstOrDefault()
                        .sessionId;

            var saveLocation = _configuration.GetSection("filestorage")["savelocation"];

            var fileNameLength = BitConverter.ToInt16(tail.Skip(4).Take(2).ToArray());

            var commandArgs = _dataTransitService
                .ConvertFromByteArray<List<string>>(args, fileNameLength);

            var fileName = Guid.NewGuid().ToString();

            if (commandArgs != null 
                && commandArgs.Count() != 0 
                && !string.IsNullOrEmpty(commandArgs.FirstOrDefault()))
            {
                fileName = commandArgs
                    .FirstOrDefault()
                    .Split(@"\")
                    .Last();
            }

            return await _fileUploaderService
                    .InitFileUploadingSessionAsync(currentServerSession, saveLocation, fileName);
        } 
    }
}
