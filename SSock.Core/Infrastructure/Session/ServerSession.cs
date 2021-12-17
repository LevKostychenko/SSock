using SSock.Core.Abstract.Infrastructure.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public static bool ExecuteInClientThread(
            string clientId,
            Func<Task> expresion)
        {
            if (SessionsIds.Any(x => x.clientId == clientId))
            {
                return ThreadPool.QueueUserWorkItem(WorkItemProc, expresion as object);
            }

            return false;
        }

        private static void WorkItemProc(object state)
        {
            var execFunc = state as Func<Task>;

            execFunc.Invoke();
        }
    }
}
