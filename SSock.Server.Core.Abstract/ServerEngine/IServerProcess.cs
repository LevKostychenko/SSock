using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SSock.Server.Core.Abstract.ServerEngine
{
    public interface IServerProcess
    {
        Task ProcessAsync(
            Socket socket, 
            Action stopServerDelegate);
    }
}
