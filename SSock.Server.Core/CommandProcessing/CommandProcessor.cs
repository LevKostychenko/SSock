using SSock.Core.Commands.Abstract.CommandsFactory;
using SSock.Server.Core.Abstract.CommandProcessing;
using System;
using System.Linq;

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

        public string Process(string command)
        {
            var parsedCommand = ParseCommand(command);
            var clientCommand = _commandFactory.CreateCommand(
                parsedCommand.command.ToUpper());

            if (clientCommand == null)
            {
                throw new NotSupportedException("Unsopported command");
            }

            return clientCommand.Execute(parsedCommand.args);
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
