using Microsoft.Extensions.DependencyInjection;
using SSock.Core.Commands;
using SSock.Server.Core;

namespace SSock.Server.Extensions
{
    internal static class AppServicesExtension
    {
        public static IServiceCollection AddDI(this IServiceCollection services)
            => services
                .AddServer()
                .AddCommands();
    }
}
