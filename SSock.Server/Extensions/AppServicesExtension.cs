using Microsoft.Extensions.DependencyInjection;
using SSock.Core.Commands;
using SSock.Core.Services;
using SSock.Server.Core;
using SSock.Server.Services;

namespace SSock.Server.Extensions
{
    internal static class AppServicesExtension
    {
        public static IServiceCollection AddDI(this IServiceCollection services)
            => services
                .AddServer()
                .AddServices()
                .AddServerServices()
                .AddCommands();
    }
}
