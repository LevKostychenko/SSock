using SSock.Core.Abstract;
using SSock.Server.Core.Abstract.CommandProcessing;
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
        private readonly ICommandProcessor _commandProcessor;

        public ServerProcess(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        public async Task ProcessAsync(
            Socket socket, 
            Action stopServerDelegate)
        {
            try
            {
                while (true)
                {
                    var clientMessage = await ReadDataAsync(socket);
                    Console.WriteLine(clientMessage);
                    string response = string.Empty;

                    try
                    {
                        response = _commandProcessor.Process(clientMessage);

                        if (IsRequestToClose(response))
                        {
                            await SendDataAsync(socket, "Connection closed.");
                            stopServerDelegate();
                            break;
                        }
                    }
                    catch (NotSupportedException ex)
                    {
                        await SendDataAsync(socket, ex.Message);
                        continue;
                    }

                    await SendDataAsync(socket, response);
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

        private bool IsRequestToClose(string serverResponse)
            => serverResponse.Equals("close", StringComparison.OrdinalIgnoreCase);
    }
}
