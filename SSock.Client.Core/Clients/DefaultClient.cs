using Microsoft.Extensions.Configuration;
using SSock.Client.Core.Abstract.Clients;
using SSock.Client.Core.Abstract.ResponseProcessing;
using SSock.Client.Domain;
using SSock.Core.Infrastructure;
using SSock.Core.Services.Abstract.Commands;
using SSock.Core.Services.Abstract.Communication;
using SSock.Core.Services.Abstract.FileUploading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IResponseProcessorFactory _responseProcessorFactory;
        private readonly IPacketService<ServerPacket, ClientPacket> _packetService;
        private readonly IDataTransitService _dataTransitService;

        public DefaultClient(
            IConfiguration configuration,
            ICommandService commandService,
            IFileUploaderService fileUploaderService,
            IDataTransitService dataTransitService,
            IResponseProcessorFactory responseProcessorFactory,
            IPacketService<ServerPacket, ClientPacket> packetService)
            : base(
                  dataTransitService,
                  configuration.GetSection("server"),
                  packetService)
        {
            _packetService = packetService;
            _fileUploaderService = fileUploaderService;
            _commandService = commandService;
            _dataTransitService = dataTransitService;
            _responseProcessorFactory = responseProcessorFactory;
        }

        protected override async Task ProcessUserCommandWithResponseAsync(
            string clientId,
            (string command, IEnumerable<string> arguments) command,
            ClientPacket receivedData,
            Ref<Socket> socket)
        {            
            if (receivedData.Status != Statuses.Ok)
            {
                Console.WriteLine(receivedData.Status);
                return;
            }

            var processor = _responseProcessorFactory
                .CreateResponseProcessor(
                    command.command,
                    socket);
            if (processor != default)
            {
                await processor.ProcessAsync(
                    command.arguments,
                    receivedData
                        .Payload
                        .Take(BitConverter.ToInt16(
                            receivedData
                            .Tail
                            .Skip(2)
                            .Take(2)
                            .ToArray())),
                    clientId);
            }

            // var parsedCommand = _commandService.ParseCommand(command.Trim(' ')[0]);

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
