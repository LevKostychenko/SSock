using Microsoft.Extensions.DependencyInjection;
using SSock.Client.Domain;
using SSock.Client.Services.Abstract;
using SSock.Core.Services.Abstract.Communication;

namespace SSock.Client.Services
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddClientServices(this IServiceCollection services)
            => services
                .AddTransient<IUploadingService, UploadingService>()
                .AddTransient<IPacketService<ServerPacket, ClientPacket>, PacketService>();
    }
}
