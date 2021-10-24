using Microsoft.Extensions.DependencyInjection;
using SSock.Client.Core;
using SSock.Core.Services;

namespace SSock.Client.Extensions
{
    internal static class AppServicesExtension
    {
        public static IServiceCollection AddDI(this IServiceCollection services)
            => services
               .AddClients()
               .AddServices();
    }
}
