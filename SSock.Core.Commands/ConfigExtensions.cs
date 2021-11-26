using Microsoft.Extensions.DependencyInjection;
using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Commands.Abstract.CommandsFactory;
using SSock.Core.Commands.AppCommands;
using SSock.Core.Commands.CommandsFactory;

namespace SSock.Core.Commands
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddCommands(this IServiceCollection services)
            => services
                .AddTransient<ICommandFactory, CommandFactory>()
                .AddTransient<ICommand, CloseCommand>()
                .AddTransient<ICommand, EchoCommand>()
                .AddTransient<ICommand, TimeCommand>()
                .AddTransient<ICommand, InitUploadCommand>()
                .AddTransient<ICommand, CommitUploadCommand>()
                .AddTransient<ICommand, ContinueUploadCommand>()            
                .AddTransient<ICommand, UploadDataCommand>();
    }
}
