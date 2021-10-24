using System.Threading.Tasks;

namespace SSock.Core.Abstract.Infrastructure.Session
{
    public interface ICache
    {
        Task<T> GetOrCreateAsync<T>(object key, T value);
    }
}
