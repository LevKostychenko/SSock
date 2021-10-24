using Microsoft.Extensions.DependencyInjection;
using SSock.Core.Services.Abstract.FileUploading;
using SSock.Core.Services.FileUploading;

namespace SSock.Core.Commands
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
            => services
                .AddTransient<IFileUploaderService, FileUploaderService>();
    }
}