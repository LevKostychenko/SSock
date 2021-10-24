using SSock.Core.Abstract.Infrastructure.Session;
using System;
using System.Collections.Generic;

namespace SSock.Core.Infrastructure.Session
{
    public static class ServerSession
    {
        public static IDictionary<string, ICache> SessionsCache { get; set; }

        public static ICollection<(string clientId, string sessionId)> SessionsIds { get; private set; }

        public static string InitNewSession(string clientId)
        {
            var newSessionId = Guid.NewGuid().ToString();

            if (SessionsIds == null)
            {
                SessionsIds = new List<(string, string)>();
            }

            SessionsIds.Add((clientId, newSessionId));

            if (SessionsCache == null)
            {
                SessionsCache = new Dictionary<string, ICache>();
            }

            SessionsCache[newSessionId] = new Cache();
            return newSessionId;
        }
    }
}
