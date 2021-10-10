using Microsoft.Extensions.DependencyInjection;

namespace SSock.Server.Extensions
{
    internal static class AppServicesExtension
    {
        public static IServiceCollection AddDI(this IServiceCollection services)
            => services;
    }
}
