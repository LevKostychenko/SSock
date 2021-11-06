using Microsoft.Extensions.DependencyInjection;
using SSock.Core.Services.Abstract.Commands;
using SSock.Core.Services.Abstract.Communication;
using SSock.Core.Services.Abstract.FileUploading;
using SSock.Core.Services.Commands;
using SSock.Core.Services.Communication;
using SSock.Core.Services.FileUploading;

namespace SSock.Core.Services
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
            => services
                .AddTransient<IFileUploaderService, FileUploaderService>()
                .AddTransient<IDataTransitService, DataTransitService>()
                .AddTransient<ICommandService, CommandService>();
    }
}