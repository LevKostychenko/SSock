using System.Threading.Tasks;

namespace SSock.Client.Core.Abstract.Clients
{
    public interface IClient
    {
        Task RunAsync();

        void Stop();
    }
}
