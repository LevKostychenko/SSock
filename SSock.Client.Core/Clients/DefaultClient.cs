﻿using Microsoft.Extensions.Configuration;
using SSock.Client.Core.Abstract.Clients;
using SSock.Client.Core.Abstract.ResponseProcessing;
using SSock.Client.Domain;
using SSock.Core.Infrastructure;
using SSock.Core.Services.Abstract.Commands;
using SSock.Core.Services.Abstract.Communication;
using SSock.Core.Services.Abstract.FileUploading;
using System;
using System.Collections.Generic;
using System.Linq;
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
                  configuration,
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
            ClientPacket receivedData)
        {            
            if (receivedData.Status != Statuses.Ok)
            {
                Console.WriteLine(receivedData.Status);
                return;
            }

            var processor = _responseProcessorFactory
                .CreateResponseProcessor(
                    command.command,
                    Client);
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
        }   

        public override void Stop()
            => throw new NotImplementedException();

        protected override ClientPacket ParsePacket(IEnumerable<byte> packet)
            => _packetService.ParsePacket(packet);
    }
}
