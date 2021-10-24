using Microsoft.Extensions.DependencyInjection;
using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Commands.Abstract.CommandsFactory;
using SSock.Core.Commands.AppCommands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSock.Core.Commands.CommandsFactory
{
    internal class CommandFactory
        : ICommandFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICommand CreateCommand(string command)
        {
            var abstractCommnds = (IEnumerable<ICommand>)_serviceProvider.GetServices(typeof(ICommand));

            switch (command)
            {
                case CommandsNames.CloseCommand:
                    {
                        return (CloseCommand)abstractCommnds
                            .Where(c => c is CloseCommand)
                            .FirstOrDefault();
                    }
                case CommandsNames.EchoCommand:
                    {
                        return (EchoCommand)abstractCommnds
                            .Where(c => c is EchoCommand)
                            .FirstOrDefault();
                    }
                case CommandsNames.TimeCommand:
                    {
                        return (TimeCommand)abstractCommnds
                            .Where(c => c is TimeCommand)
                            .FirstOrDefault();
                    }
                case CommandsNames.UploadCommand:
                    {
                        return (UploadFileCommand)abstractCommnds
                            .Where(c => c is UploadFileCommand)
                            .FirstOrDefault();
                    }
                default:
                    {
                        return default;
                    }
            }
        }
    }
}
