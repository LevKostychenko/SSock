namespace SSock.Server.Domain
{
    public class ServerPacket
    {
        public string Command { get; set; }

        public string ClientId { get; set; }

        public byte[] Payload { get; set; }

        public byte[] Tail { get; set; }
    }
}
