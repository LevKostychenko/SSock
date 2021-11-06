﻿using System.Collections.Generic;

namespace SSock.Core.Services.Abstract.Communication
{
    public interface IPacketService<TIn, TOut>
    {
        IEnumerable<byte> CreatePacket(TIn packetEntity);

        TOut ParsePacket(IEnumerable<byte> packet);

        string GetCommandArgumnets(string command);
    }
}
