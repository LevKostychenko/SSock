using Microsoft.Extensions.DependencyInjection;
using SSock.Server.Core.Abstract.CommandProcessing;
using SSock.Server.Core.Abstract.ServerEngine;
using SSock.Server.Core.CommandProcessing;
using SSock.Server.Core.ServerEngine;

namespace SSock.Server.Core
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddServer(this IServiceCollection services)
            => services
                .AddTransient<IServerRunner, ServerRunner>()
                .AddTransient<ICommandProcessor, CommandProcessor>()
                .AddScoped<IServerProcess, ServerProcess>();
    }
}
