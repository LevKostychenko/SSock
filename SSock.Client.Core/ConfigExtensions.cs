using Microsoft.Extensions.DependencyInjection;
using SSock.Client.Core.Abstract.Clients;
using SSock.Client.Core.Abstract.ResponseProcessing;
using SSock.Client.Core.Clients;
using SSock.Client.Core.ResponseProcessing;

namespace SSock.Client.Core
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddClients(this IServiceCollection services)
            => services.AddTransient<IClient, DefaultClient>()
                .AddTransient<IResponseProcessorFactory, ResponseProcessorFactory>();
    }
}
