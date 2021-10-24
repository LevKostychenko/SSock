using SSock.Core.Abstract;
using SSock.Core.Infrastructure.Session;
using SSock.Server.Core.Abstract.CommandProcessing;
using SSock.Server.Core.Abstract.ServerEngine;
using System;
using System.Linq;
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
                    var (isNewClient, clientId) = IsNewClientConnected(clientMessage);

                    if (isNewClient && !string.IsNullOrEmpty(clientId))
                    {
                        await NewClientConnectedAsync(clientId, socket);                        
                        continue;
                    }

                    Console.WriteLine($"{clientMessage}");
                    string response = string.Empty;

                    try
                    {
                        response = await _commandProcessor.ProcessAsync(clientMessage);

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

        private (bool, string) IsNewClientConnected(string message)
        {
            var messageParts = message.Split(" ");

            if (messageParts.Length == 2 
                && messageParts[0] == INIT_MESSAGE 
                && !string.IsNullOrEmpty(messageParts[1]))
            {
                return (true, messageParts[1]);
            }

            return (false, string.Empty);
        }

        private async Task NewClientConnectedAsync(string clientId, Socket socket)
        {
            ServerSession.InitNewSession(clientId);
            Console.WriteLine($"Client with ID {clientId} is connected.");
            await SendDataAsync(socket, CONNECTED_MESSAGE);
        }
    }
}
