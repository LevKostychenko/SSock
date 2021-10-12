using SSock.Core.Abstract;
using SSock.Server.Core.Abstract.ServerEngine;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SSock.Server.Core.ServerEngine
{
    internal class ServerProcess
        : BaseProcess,
        IServerProcess
    {
        public async Task ProcessAsync(Socket socket)
        {
            try
            {
                while (true)
                {
                    var clientMessage = await ReadDataAsync(socket);
                    Console.WriteLine(clientMessage);

                    await SendDataAsync(socket, "Message received");
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
            finally
            {
                if (socket != null)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
            }
        }
    }
}
