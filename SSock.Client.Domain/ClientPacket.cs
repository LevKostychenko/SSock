namespace SSock.Client.Domain
{
    public class ClientPacket
    {
        public string Status { get; set; }

        public byte[] Payload { get; set; }

        public byte[] Tail { get; set; }
    }
}
