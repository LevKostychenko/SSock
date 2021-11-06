using System.Collections.Generic;

namespace SSock.Server.Domain
{
    public class ClientPacket
    {
        public string Status { get; set; }

        public IEnumerable<byte> Payload { get; set; }

        public IEnumerable<byte> Tail { get; set; }
    }
}
