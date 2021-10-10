using Microsoft.Extensions.DependencyInjection;
using SSock.Client.Core;

namespace SSock.Client.Extensions
{
    internal static class AppServicesExtension
    {
        public static IServiceCollection AddDI(this IServiceCollection services)
            => services
               .AddClients();
    }
}
