using SSock.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SSock.Core
{
    public abstract class BaseProcess<T>
    {
        protected const int READ_CHUNK_SIZE = 2;
        protected abstract T ParsePacket(IEnumerable<byte> packet);

        protected Ref<UdpClient> Sender { get; set; }
        protected Ref<UdpClient> Receiver { get; set; }

        protected BaseProcess(
            int localSenderPort,
            int localReceiverPort)
        {
            Sender = new Ref<UdpClient>();
            Receiver = new Ref<UdpClient>();

            Sender.Value = new UdpClient(localSenderPort);
            Receiver.Value = new UdpClient(localReceiverPort);
        }

        protected void LogError(string error)
            => Console.WriteLine("Error: " + error);
    }
}
