using SSock.Core.Services.Abstract.Commands;
using System;
using System.Linq;

namespace SSock.Core.Services.Commands
{
    internal class CommandService
        : ICommandService
    {
        public (string command, string[] args) ParseCommand(string command)
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
