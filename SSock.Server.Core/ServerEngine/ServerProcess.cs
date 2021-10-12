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
        public async Task ProcessAsync(TcpClient client)
        {
            NetworkStream stream = null;

            try
            {
                stream = client.GetStream();

                while (true)
                {
                    var clientMessage = await ReadDataAsync(stream);
                    Console.WriteLine(clientMessage);

                    await SendDataAsync(stream, "Message received");
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }

                if (client != null)
                {
                    client.Close();
                }
            }
        }
    }
}
