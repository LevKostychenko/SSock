using Microsoft.Extensions.Configuration;
using SSock.Client.Core.Abstract.Clients;
using SSock.Client.Domain;
using SSock.Core.Commands;
using SSock.Core.Services.Abstract.Commands;
using SSock.Core.Services.Abstract.Communication;
using SSock.Core.Services.Abstract.FileUploading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SSock.Client.Core.Clients
{
    internal class DefaultClient
        : BaseClientProcess,
        IClient
    {
        private readonly ICommandService _commandService;
        private readonly IFileUploaderService _fileUploaderService;
        private readonly IPacketService<ServerPacket, ClientPacket> _packetService;

        public DefaultClient(
            IConfiguration configuration,
            ICommandService commandService,
            IFileUploaderService fileUploaderService,
            IPacketService<ServerPacket, ClientPacket> packetService)
            : base(
                  configuration.GetSection("server"),
                  packetService)
        {
            _packetService = packetService;
            _fileUploaderService = fileUploaderService;
            _commandService = commandService;
        }

        protected override async Task ProcessUserCommandWithResponseAsync(
            string clientId,
            string command,
            ClientPacket receivedData,
            Socket socket)
        {
            //var parsedCommand = _commandService.ParseCommand(command.Trim(' ')[0]);

            //if (parsedCommand.command.Equals(
            //    CommandsNames.InitUploadCommand, 
            //    StringComparison.OrdinalIgnoreCase))
            //{                
            //    if (string.IsNullOrEmpty(parsedCommand.args[0]))
            //    {
            //        throw new Exception("File path does not presents ath the command arguments");
            //    }

            //    await using var stream = File.Open(parsedCommand.args[0], FileMode.Open);

            //    var fileBytes = await _fileUploaderService
            //        .GetFileBytesAsync(stream);

            //    var bytesString = Encoding.Unicode.GetString(fileBytes, 0, fileBytes.Length);

            //    if (!string.IsNullOrEmpty(receivedData))
            //    {
            //        await SendDataAsync(socket, 
            //            $"{CommandsNames.UploadDataCommand} " +
            //            $"{receivedData} " +
            //            $"{Path.GetFileName(parsedCommand.args[0])} " +
            //            $"{bytesString} ");
            //    }
            //}
        }

        public override async Task RunAsync()
            => await base.RunAsync();        

        public override void Stop()
            => throw new NotImplementedException();

        protected override ClientPacket ParsePacket(IEnumerable<byte> packet)
            => _packetService.ParsePacket(packet);
    }
}
