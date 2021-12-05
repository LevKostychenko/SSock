using Microsoft.Extensions.Configuration;
using SSock.Client.Core.Abstract.ResponseProcessing;
using SSock.Client.Domain;
using SSock.Client.Services.Abstract;
using SSock.Core.Abstract.Infrastructure;
using SSock.Core.Commands;
using SSock.Core.Infrastructure;
using SSock.Core.Infrastructure.Session;
using SSock.Core.Services.Abstract.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SSock.Client.Core.ResponseProcessing
{
    internal class InitUploadResponseProcessor
        : IResponseProcessor
    {
        protected const string UPLOADING_FILE_PATH_KEY = "UPLOADING_FILE_PATH";
        private const int READ_CHUNK_SIZE = 2;

        private readonly IServiceProvider _serviceProvider;

        private Ref<UdpClient> _client;

        public InitUploadResponseProcessor(
            IServiceProvider serviceProvider,
            Ref<UdpClient> client)
        {
            _serviceProvider = serviceProvider;
            _client = client;
        }

        public async Task<object> ProcessAsync(
            IEnumerable<string> arguments,
            IEnumerable<byte> payload,
            string clientId)
        {
            var dataTransitService = (IDataTransitService)_serviceProvider
                   .GetService(typeof(IDataTransitService));
            var config = (IConfiguration)_serviceProvider
               .GetService(typeof(IConfiguration));

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

            await UploadFileAsync(
                clientId,
                uploadingHash,
                filePath,
                chunkSize);

            var commitResponse = await CommitUploadingAsync(
                clientId,
                uploadingHash,
                chunkSize);

            if (dataTransitService
                .ConvertFromByteArray<int>(commitResponse.Payload, commitResponse.Payload.Length) == 1)
            {
                Console.WriteLine("Uploaded");
            }
            else
            {
                Console.WriteLine("Cannot commit");
            }

            return "Uploaded";
        }

        private IEnumerable<byte> GetUploadPacket(
            string clientId,
            string uploadingHash,
            byte[] chunk)
        {
            var packetService = (IPacketService<ServerPacket, ClientPacket>)_serviceProvider
                 .GetService(typeof(IPacketService<ServerPacket, ClientPacket>));
            var uploadingService = (IUploadingService)_serviceProvider
                    .GetService(typeof(IUploadingService));

            return packetService.CreatePacket(new ServerPacket
            {
                Command = CommandsNames.UPLOAD_DATA_COMMAND,
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
        }

        private IEnumerable<byte> GetCommitPacket(
            string clientId,
            string uploadingHash)
        {
            var packetService = (IPacketService<ServerPacket, ClientPacket>)_serviceProvider
                .GetService(typeof(IPacketService<ServerPacket, ClientPacket>));

            return packetService.CreatePacket(new ServerPacket
            {
                Command = CommandsNames.COMMIT_UPLOAD_COMMAND,
                ClientId = clientId,
                Payload = Encoding.Unicode.GetBytes(uploadingHash),
                PayloadParts = new List<int>
                        {
                            Encoding.Unicode.GetByteCount(uploadingHash)
                        }
            });
        }

        private async Task<ClientPacket> CommitUploadingAsync(
            string clientId,
            string uploadingHash,
            int chunkSize)
        {
            var packetService = (IPacketService<ServerPacket, ClientPacket>)_serviceProvider
                .GetService(typeof(IPacketService<ServerPacket, ClientPacket>));
            var dataTransitService = (IDataTransitService)_serviceProvider
                .GetService(typeof(IDataTransitService));

            var commitPacket = GetCommitPacket(clientId, uploadingHash);

            await dataTransitService.SendDataAsync(_client, commitPacket);
            return await dataTransitService
                    .ReadDataAsync(
                    _client,
                    chunkSize,
                    p => packetService.ParsePacket(p));
        }

        private async Task UploadFileAsync(
            string clientId,
            string uploadingHash,
            string filePath,
            int chunkSize)
        {
            var dataTransitService = (IDataTransitService)_serviceProvider
                .GetService(typeof(IDataTransitService));
            var packetService = (IPacketService<ServerPacket, ClientPacket>)_serviceProvider
                .GetService(typeof(IPacketService<ServerPacket, ClientPacket>));

            var chunk = new byte[chunkSize];

            using var progress = new ProgressBar();
            using (var fileReader = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read))
            {
                using var reader = new BinaryReader(fileReader);
                int bytesToRead = (int)fileReader.Length;

                do
                {
                    chunk = reader.ReadBytes(chunkSize);
                    bytesToRead -= chunkSize;

                    var packet = GetUploadPacket(clientId, uploadingHash, chunk);

                    await dataTransitService.SendDataAsync(_client, packet);
                    ClientPacket response = null;

                    try
                    {
                        response = await dataTransitService
                            .ReadDataAsync(
                                _client,
                                chunkSize,
                                p => packetService.ParsePacket(p));
                    }
                    catch (Exception ex)
                    {
                        await ReconnectSocketAsync(clientId);
                        await dataTransitService.SendDataAsync(_client, packet);
                        response = await dataTransitService
                            .ReadDataAsync(
                                _client,
                                chunkSize,
                                p => packetService.ParsePacket(p));
                    }

                    progress.Report(
                        (double)Decimal.Divide(fileReader.Length - bytesToRead, fileReader.Length));

                    if (GetFileNextOffset(
                        response.Payload,
                        dataTransitService) == 0)
                    {
                        Console.WriteLine("Already uploaded");
                        return;
                    }

                } while (bytesToRead > 0);
            }
        }

        private async Task ReconnectSocketAsync(
            string clientId)
        {
            var config = (IConfiguration)_serviceProvider
                .GetService(typeof(IConfiguration));

            var (port, address) = (
                config.GetSection("server")["port"],
                config.GetSection("server")["address"]);

            var ipPoint = new IPEndPoint(
                IPAddress.Parse(address),
                Int32.Parse(port));

            var newClient = new UdpClient();

            await ConnectSocketAsync(
                newClient,
                ipPoint,
                clientId);

            _client.Value = newClient;
        }

        private async Task<bool> ConnectSocketAsync(
            UdpClient client,
            IPEndPoint ipPoint,
            string clientId)
        {
            var dataTransitService = (IDataTransitService)_serviceProvider
               .GetService(typeof(IDataTransitService));
            var packetService = (IPacketService<ServerPacket, ClientPacket>)_serviceProvider
                 .GetService(typeof(IPacketService<ServerPacket, ClientPacket>));

            Console.WriteLine("Connection to the server...");
            client.Connect(ipPoint);

            await dataTransitService.SendDataAsync(
                        client,
                        packetService.CreatePacket(
                            new ServerPacket
                            {
                                Command = CommandsNames.INIT_COMMAND,
                                ClientId = clientId
                            }));

            var receivedData = await dataTransitService.ReadDataAsync(
                client,
                READ_CHUNK_SIZE,
                x => packetService.ParsePacket(x));

            if (receivedData.Status == Statuses.Connected)
            {
                Console.WriteLine("Connected successfully");
                return true;
            }

            return false;
        }

        private long GetFileNextOffset(
            IEnumerable<byte> payload,
            IDataTransitService dataTransitService)
            => dataTransitService.ConvertFromByteArray<long>(payload, payload.Count());
    }
}
