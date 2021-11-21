using SSock.Server.Domain;
using System.Threading.Tasks;

namespace SSock.Server.Core.Abstract.CommandProcessing
{
    public interface ICommandProcessor
    {
        Task<object> ProcessAsync(ServerPacket packet);
    }
}
