using SSock.Client.Domain;
using SSock.Core.Infrastructure.Extensions;
using SSock.Core.Services.Abstract.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSock.Client.Services
{
    internal class PacketService
        : IPacketService<ServerPacket, ClientPacket>
    {
        private const int COMMAND_LENGTH = 64;
        private const int CLIENT_ID_LENGTH = 128;
        private const int PAYLOAD_LENGTH = 1024;
        private const int TAIL_LENGTH = 64;

        private const int STATUS_CHUNK_LENGTH = 64;
        private const int PAYLOAD_CHUNK_LENGTH = 1024;
        private const int TAIL_CHUNK_LENGTH = 64;

        private readonly IDataTransitService _dataTransitService;

        public PacketService(IDataTransitService dataTransitService)
        {
            _dataTransitService = dataTransitService;
        }

        public IEnumerable<byte> CreatePacket(
            ServerPacket serverPacket)
        {
            var packet = new byte[
                COMMAND_LENGTH 
                + CLIENT_ID_LENGTH 
                + PAYLOAD_LENGTH
                + TAIL_LENGTH];

            var commandBytes = Encoding.Unicode.GetBytes(serverPacket.Command);
            if (commandBytes.Length > COMMAND_LENGTH)
            {
                throw new Exception("Command is too large.");
            }

            var clientIdBytes = Encoding.Unicode.GetBytes(serverPacket.ClientId);
            if (clientIdBytes.Length > CLIENT_ID_LENGTH)
            {
                throw new Exception("Client Id is too large.");
            }

            var tailBytes = BuildTail(
                commandBytes,
                clientIdBytes,                
                serverPacket.Payload,
                serverPacket.PayloadParts);

            commandBytes = _dataTransitService.AppendBytes(commandBytes, COMMAND_LENGTH);
            clientIdBytes = _dataTransitService.AppendBytes(clientIdBytes, CLIENT_ID_LENGTH);
            byte[] payloadBytes;

            if (serverPacket.Payload == null)
            {
                payloadBytes = new byte[PAYLOAD_LENGTH];
            }
            else
            {
                if (serverPacket.Payload.Count() > PAYLOAD_LENGTH)
                {
                    throw new Exception("Payload is too large.");
                }

                payloadBytes = _dataTransitService.AppendBytes(
                    serverPacket.Payload.ToArray(),
                    PAYLOAD_LENGTH);
            }

            packet = commandBytes
                .Concat(clientIdBytes)
                .Concat(payloadBytes)
                .Concat(tailBytes)
                .ToArray();

            return packet;
        }

        public (string command, IEnumerable<string> arguments) GetCommandParts(
            string command)
        {
            var commandPart = command.Split(' ')[0];
            var arguments = command.Split(' ').Skip(1);

            return (commandPart, arguments.Any() ? arguments : null);
        }

        public ClientPacket ParsePacket(IEnumerable<byte> packet)
        {
            if (packet == null 
                || !packet.Any())
            {
                throw new ArgumentNullException("Packet is null or empty");
            }

            var stausBytes = packet.Take(STATUS_CHUNK_LENGTH);
            var payloadBytes = packet
                .Skip(STATUS_CHUNK_LENGTH)
                .Take(PAYLOAD_CHUNK_LENGTH);
            var tailBytes = packet
                .TakeLast(TAIL_CHUNK_LENGTH);

            var tail = ParseTail(tailBytes.ToArray());

            return new ClientPacket
            {
                Status = stausBytes
                    .Take(tail.statusLength)
                    .BytesToString(),
                Payload = payloadBytes.ToArray(),
                Tail = tailBytes.ToArray()
            };
        }

        private (short statusLength, short payloadLength) ParseTail(byte[] tail)
        {
            var statusLength = BitConverter.ToInt16(tail
                .Take(2)
                .ToArray());
            var payloadLength = BitConverter.ToInt16(tail
                .Skip(2)
                .Take(2)
                .ToArray());

            // TODO: Also process payload parts
            return (statusLength, payloadLength);
        }

        private byte[] BuildTail(
            byte[] commandBytes,
            byte[] clientIdBytes,
            IEnumerable<byte> payload = null,
            IEnumerable<int> payloadParts = null)
        {
            unchecked
            {
                var commandLength = (short)commandBytes.Length;
                var clientIdLength = (short)clientIdBytes.Length;
                var payloadLength = (short)(payload == null ? 0 : payload.Count());

                var payloadPartsLength = new List<byte>();

                if (payloadParts != null && payloadParts.Count() > 0)
                {
                    foreach (var part in payloadParts)
                    {
                        payloadPartsLength.AddRange(BitConverter.GetBytes((short)part));
                    }
                }

                return _dataTransitService.AppendBytes(
                    BitConverter.GetBytes(commandLength)
                        .Concat(BitConverter.GetBytes(clientIdLength))
                        .Concat(BitConverter.GetBytes(payloadLength))
                        .Concat(payloadPartsLength)
                        .ToArray(),
                    TAIL_LENGTH);
            }           
        }
    }
}
