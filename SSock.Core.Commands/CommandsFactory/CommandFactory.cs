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
            var abstractCommnds = (IEnumerable<ICommand>)_serviceProvider
                .GetServices(typeof(ICommand));

            switch (command)
            {
                case CommandsNames.CLOSE_COMMAND:
                    {
                        return (CloseCommand)abstractCommnds
                            .Where(c => c is CloseCommand)
                            .FirstOrDefault();
                    }
                case CommandsNames.ECHO_COMMAND:
                    {
                        return (EchoCommand)abstractCommnds
                            .Where(c => c is EchoCommand)
                            .FirstOrDefault();
                    }
                case CommandsNames.TIME_COMMAND:
                    {
                        return (TimeCommand)abstractCommnds
                            .Where(c => c is TimeCommand)
                            .FirstOrDefault();
                    }
                case CommandsNames.INIT_UPLOAD_COMMAND:
                    {
                        return (InitUploadCommand)abstractCommnds
                            .Where(c => c is InitUploadCommand)
                            .FirstOrDefault();
                    }
                case CommandsNames.UPLOAD_DATA_COMMAND:
                    {
                        return (UploadDataCommand)abstractCommnds
                            .Where(c => c is UploadDataCommand)
                            .FirstOrDefault();
                    }
                case CommandsNames.COMMIT_UPLOAD_COMMAND:
                    {
                        return (CommitUploadCommand)abstractCommnds
                            .Where(c => c is CommitUploadCommand)
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
