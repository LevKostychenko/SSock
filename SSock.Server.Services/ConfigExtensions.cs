using Microsoft.Extensions.DependencyInjection;
using SSock.Core.Services.Abstract.Communication;
using SSock.Server.Domain;

namespace SSock.Server.Services
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddServerServices(this IServiceCollection services)
            => services
                .AddTransient<IPacketService<ClientPacket, ServerPacket>, PacketService>();
    }
}
