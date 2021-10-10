using Microsoft.Extensions.DependencyInjection;

namespace SSock.Core.Infrastructure
{
    public static class ServiceBuilder
    {
        public static IServiceCollection BuildServiceCollection()
            => new ServiceCollection();
    }
}
