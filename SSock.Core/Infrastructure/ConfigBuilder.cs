using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace SSock.Core.Infrastructure
{
    public static class ConfigBuilder
    {
        public static IServiceCollection AddConfiguration(
            this IServiceCollection services, 
            string configFilePath, 
            string[] args = null)
        {
            var config = new ConfigurationBuilder();

            if (!string.IsNullOrEmpty(configFilePath))
            {
                config
                .AddJsonFile(configFilePath, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            }

            if (args != null && args.Length > 0)
            {
                config
                .AddCommandLine(args);
            }

            return services.AddSingleton<IConfiguration>(config.Build());
        }
    }
}
