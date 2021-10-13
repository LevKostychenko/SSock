using Microsoft.Extensions.DependencyInjection;
using SSock.Core.Commands.Abstract.CommandsFactory;
using SSock.Core.Commands.CommandsFactory;

namespace SSock.Core.Commands
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddCommands(this IServiceCollection services)
            => services
                .AddTransient<ICommandFactory, CommandFactory>();
    }
}
