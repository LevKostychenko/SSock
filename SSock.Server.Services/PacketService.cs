using SSock.Core.Infrastructure.Extensions;
using SSock.Core.Services.Abstract.Communication;
using SSock.Server.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSock.Server.Services
{
    internal class PacketService
        : IPacketService<ClientPacket, ServerPacket>
    {
        private const int STATUS_LENGTH = 64;
        private const int PAYLOAD_LENGTH = 1024;
        private const int TAIL_LENGTH = 64;

        private const int COMMAND_CHUNK_LENGTH = 64;
        private const int CLIENT_ID_CHUNK_LENGTH = 128;
        private const int PAYLOAD_CHUNK_LENGTH = 1024;
        private const int TAIL_CHUNK_LENGTH = 64;

        private readonly IDataTransitService _dataTransitService;

        public PacketService(IDataTransitService dataTransitService)
        {
            _dataTransitService = dataTransitService;
        }

        public IEnumerable<byte> CreatePacket(ClientPacket packetEntity)
        {
            var packet = new byte[
                STATUS_LENGTH
                + PAYLOAD_LENGTH
                + TAIL_LENGTH];

            var statusBytes = Encoding.Unicode.GetBytes(packetEntity.Status);
            if (statusBytes.Length > STATUS_LENGTH)
            {
                throw new Exception("Status is too large.");
            }

            var tailBytes = BuildTail(
                statusBytes,
                packetEntity.Payload);

            statusBytes = _dataTransitService.AppendBytes(statusBytes, STATUS_LENGTH);
            byte[] payloadBytes;

            if (packetEntity.Payload == null)
            {
                payloadBytes = new byte[PAYLOAD_LENGTH];
            }
            else
            {
                if (packetEntity.Payload.Count() > PAYLOAD_LENGTH)
                {
                    throw new Exception("Payload is too large.");
                }

                payloadBytes = _dataTransitService.AppendBytes(
                    packetEntity.Payload.ToArray(),
                    PAYLOAD_LENGTH);
            }

            packet = statusBytes
                .Concat(payloadBytes)
                .Concat(tailBytes)
                .ToArray();

            return packet;
        }

        public ServerPacket ParsePacket(IEnumerable<byte> packet)
        {
            if (packet == null
                || !packet.Any())
            {
                throw new ArgumentNullException("Packet is null or empty");
            }

            var commandBytes = packet.Take(COMMAND_CHUNK_LENGTH);
            var clientIdBytes = packet
                .Skip(COMMAND_CHUNK_LENGTH)
                .Take(CLIENT_ID_CHUNK_LENGTH);
            var payloadBytes = packet
                .Skip(COMMAND_CHUNK_LENGTH + CLIENT_ID_CHUNK_LENGTH)
                .Take(PAYLOAD_CHUNK_LENGTH);
            var tailBytes = packet
                .TakeLast(TAIL_CHUNK_LENGTH);

            var tail = ParseTail(tailBytes.ToArray());

            return new ServerPacket
            {
                Command = commandBytes
                    .Take(tail.commandLength)
                    .BytesToString(),
                ClientId = clientIdBytes
                    .Take(tail.clientIdLength)
                    .BytesToString(),
                Payload = payloadBytes
                    .Take(tail.payloadLength)
                    .ToArray(),
                Tail = tailBytes.ToArray()
            };
        }

        private (short commandLength, short clientIdLength, short payloadLength) ParseTail(byte[] tail)
        {
            var commandLength = BitConverter.ToInt16(tail.Take(2).ToArray());
            var clientIdLength = BitConverter.ToInt16(tail.Skip(2).Take(2).ToArray());
            var payloadLength = BitConverter.ToInt16(tail.Skip(4).Take(2).ToArray());

            // TODO: Also process payload parts
            return (commandLength, clientIdLength, payloadLength);
        }

        private byte[] BuildTail(
            byte[] statusBytes,
            IEnumerable<byte> payload = null)
        {
            unchecked
            {
                var statusLength = (short)statusBytes.Length;
                var payloadLength = (short)(payload == null ? 0 : payload.Count());

                return _dataTransitService.AppendBytes(
                    BitConverter.GetBytes(statusLength)
                        .Concat(BitConverter.GetBytes(payloadLength))
                        .ToArray(),
                    TAIL_LENGTH);
            }
        }

        public (string command, IEnumerable<string> arguments) GetCommandParts(string command)
        {
            throw new NotImplementedException();
        }
    }
}
