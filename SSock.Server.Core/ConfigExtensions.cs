using Microsoft.Extensions.DependencyInjection;
using SSock.Server.Core.Abstract.ServerEngine;
using SSock.Server.Core.ServerEngine;

namespace SSock.Server.Core
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddServer(this IServiceCollection services)
            => services
                .AddTransient<IServerRunner, ServerRunner>()
                .AddScoped<IServerProcess, ServerProcess>();
    }
}
