using System.Collections.Generic;

namespace SSock.Client.Domain
{
    public class ServerPacket
    {
        public string Command { get; set; }

        public string ClientId { get; set; }

        public IEnumerable<byte> Payload { get; set; }
    }
}
