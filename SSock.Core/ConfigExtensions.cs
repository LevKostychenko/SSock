using Microsoft.Extensions.DependencyInjection;
using SSock.Core.Abstract.Infrastructure.Helpers;
using SSock.Core.Infrastructure.Helpers;

namespace SSock.Core
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
            => services
                .AddTransient<ITimeOutHelper, TimeOutHelper>();
    }
}
