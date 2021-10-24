using System.Threading.Tasks;

namespace SSock.Server.Core.Abstract.ServerEngine
{
    public interface IServerRunner
    {
        Task RunAsync();
    }
}
