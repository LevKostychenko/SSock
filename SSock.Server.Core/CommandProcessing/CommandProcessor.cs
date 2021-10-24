using SSock.Core.Commands.Abstract.CommandsFactory;
using SSock.Server.Core.Abstract.CommandProcessing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SSock.Server.Core.CommandProcessing
{
    internal class CommandProcessor
        : ICommandProcessor
    {
        private readonly ICommandFactory _commandFactory;

        public CommandProcessor(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
        }

        public async Task<string> ProcessAsync(string command)
        {
            var parsedCommand = ParseCommand(command);
            var clientCommand = _commandFactory.CreateCommand(
                parsedCommand.command.ToUpper());

            if (clientCommand == null)
            {
                throw new NotSupportedException("Unsopported command");
            }

            var clientId = parsedCommand.args.Last();
            return await clientCommand.ExecuteAsync(
                parsedCommand.args, 
                clientId);
        }

        private (string command, string[] args ) ParseCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new Exception("Unsopported command");
            }

            var commandParts = command.Split(" ");

            return (commandParts[0], commandParts.Skip(1).ToArray());
        }
    }
}
