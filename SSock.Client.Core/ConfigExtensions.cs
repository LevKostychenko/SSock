using Microsoft.Extensions.DependencyInjection;
using SSock.Client.Core.Abstract.Clients;
using SSock.Client.Core.Clients;

namespace SSock.Client.Core
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddClients(this IServiceCollection services)
            => services.AddTransient<IClient, DefaultClient>();
    }
}
