using SSock.Core.Commands.Abstract.CommandsFactory;
using SSock.Core.Services.Abstract.Commands;
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
        private readonly ICommandService _commandService;

        public CommandProcessor(
            ICommandFactory commandFactory,
            ICommandService commandService)
        {
            _commandFactory = commandFactory;
            _commandService = commandService;
        }

        public async Task<string> ProcessAsync(string command)
        {
            var parsedCommand = _commandService.ParseCommand(command);
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
    }
}
