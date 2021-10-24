using Microsoft.Extensions.Configuration;
using SSock.Client.Core.Abstract.Clients;
using SSock.Core.Commands;
using SSock.Core.Services.Abstract.Commands;
using SSock.Core.Services.Abstract.FileUploading;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SSock.Client.Core.Clients
{
    internal class DefaultClient
        : BaseClient,
        IClient
    {
        private readonly ICommandService _commandService;
        private readonly IFileUploaderService _fileUploaderService;

        public DefaultClient(
            IConfiguration configuration,
            ICommandService commandService,
            IFileUploaderService fileUploaderService)
            : base(configuration.GetSection("server"))
        {
            _fileUploaderService = fileUploaderService;
            _commandService = commandService;
        }

        protected override async Task ProcessUserCommandWithResponseAsync(
            string command,
            string receivedData,
            Socket socket)
        {
            var parsedCommand = _commandService.ParseCommand(command);

            if (parsedCommand.command.Equals(
                CommandsNames.InitUploadCommand, 
                StringComparison.OrdinalIgnoreCase))
            {                
                if (string.IsNullOrEmpty(parsedCommand.args[0]))
                {
                    throw new Exception("File path does not presents ath the command arguments");
                }

                await using var stream = File.Open(parsedCommand.args[0], FileMode.Open);

                var fileBytes = await _fileUploaderService
                    .GetFileBytesAsync(stream);

                var bytesString = Encoding.Unicode.GetString(fileBytes, 0, fileBytes.Length);

                if (!string.IsNullOrEmpty(receivedData))
                {
                    await SendDataAsync(socket, 
                        $"{CommandsNames.UploadDataCommand} " +
                        $"{receivedData} " +
                        $"{Path.GetFileName(parsedCommand.args[0])} " +
                        $"{bytesString}");
                }
            }
        }

        public override async Task RunAsync()
            => await base.RunAsync();        

        public override void Stop()
            => throw new NotImplementedException();
    }
}
