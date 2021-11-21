﻿using Microsoft.Extensions.Configuration;
using SSock.Client.Core.Abstract.ResponseProcessing;
using SSock.Client.Domain;
using SSock.Client.Services.Abstract;
using SSock.Core.Abstract.Infrastructure;
using SSock.Core.Commands;
using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SSock.Client.Core.ResponseProcessing
{
    internal class InitUploadResponseProcessor
        : IResponseProcessor
    {
        protected const string UPLOADING_FILE_PATH_KEY = "UPLOADING_FILE_PATH";

        private readonly IServiceProvider _serviceProvider;

        private readonly Socket _socket;

        public InitUploadResponseProcessor(
            IServiceProvider serviceProvider,
            Socket socket)
        {
            _serviceProvider = serviceProvider;
            _socket = socket;
        }

        // TODO: Refactor this method
        public new async Task<object> ProcessAsync(
            IEnumerable<string> arguments,
            IEnumerable<byte> payload,
            string clientId)
        {
            var packetService = (IPacketService<ServerPacket, ClientPacket>)_serviceProvider
                 .GetService(typeof(IPacketService<ServerPacket, ClientPacket>));
            var uploadingService = (IUploadingService)_serviceProvider
                    .GetService(typeof(IUploadingService));
            var config = (IConfiguration)_serviceProvider
                   .GetService(typeof(IConfiguration));
            var dataTransitService = (IDataTransitService)_serviceProvider
                   .GetService(typeof(IDataTransitService));

            var uploadingHash = dataTransitService
                    .ConvertFromByteArray<string>(
                        payload,
                        payload.Count());

            if (string.IsNullOrEmpty(uploadingHash))
            {
                throw new NullReferenceException("Uploading hash is null or empty");
            }

            var filePath = arguments.FirstOrDefault();

            if (string.IsNullOrEmpty(filePath))
            {
                throw new NullReferenceException("File path is null or empty");
            }

            var newSessionId = ClientSession.InitNewSession(clientId);
            await ClientSession
                .SessionsCache[newSessionId]
                .GetOrCreateAsync(UPLOADING_FILE_PATH_KEY, filePath);            

            var chunkSize = Int32.Parse(config["chunkSize"]);
            var chunk = new byte[chunkSize];

            using (var fileReader = new FileStream(
                filePath, 
                FileMode.Open, 
                FileAccess.Read))
            {
                using var progress = new ProgressBar();
                using var reader = new BinaryReader(fileReader);
                int bytesToRead = (int)fileReader.Length;

                do
                {
                    chunk = reader.ReadBytes(chunkSize);
                    bytesToRead -= chunkSize;

                    var packet = packetService.CreatePacket(new ServerPacket
                    {
                        Command = CommandsNames.UploadDataCommand,
                        ClientId = clientId,
                        Payload = uploadingService.GetUploadDataPayload(
                            chunk,
                            uploadingHash),
                        PayloadParts = new List<int>
                        {
                            Encoding.Unicode.GetByteCount(uploadingHash),
                            chunk.Length
                        }
                    });

                    await dataTransitService.SendDataAsync(_socket, packet);
                    var response = await dataTransitService
                        .ReadDataAsync(
                        _socket,
                        chunkSize,
                        p => packetService.ParsePacket(p));

                    progress.Report(
                        (double)Decimal.Divide(fileReader.Length - bytesToRead, fileReader.Length));

                    if (GetFileNextOffset(
                        response.Payload,
                        dataTransitService) == 0)
                    {
                        Console.WriteLine("Already uploaded");
                        return "Already uploaded";
                    }

                } while (bytesToRead > 0);

                var commitPacket = packetService.CreatePacket(new ServerPacket
                {
                    Command = CommandsNames.CommitUploadCommand,
                    ClientId = clientId,
                    Payload = Encoding.Unicode.GetBytes(uploadingHash),
                    PayloadParts = new List<int>
                        {
                            Encoding.Unicode.GetByteCount(uploadingHash)
                        }
                });

                await dataTransitService.SendDataAsync(_socket, commitPacket);
                var commitResponse = await dataTransitService
                        .ReadDataAsync(
                        _socket,
                        chunkSize,
                        p => packetService.ParsePacket(p));

                if (dataTransitService
                    .ConvertFromByteArray<int>(commitResponse.Payload, commitResponse.Payload.Length) == 1)
                {
                    progress.Report((double)1);
                    Console.WriteLine("Uploaded");
                }
                else
                {
                    Console.WriteLine("Cannot commit");
                }
            }

            return "Uploaded";
        }

        private long GetFileNextOffset(
            IEnumerable<byte> payload,
            IDataTransitService dataTransitService)
            => dataTransitService.ConvertFromByteArray<long>(payload, payload.Count());
    }
}
