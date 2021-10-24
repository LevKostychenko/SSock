using System.Threading.Tasks;

namespace SSock.Server.Core.Abstract.CommandProcessing
{
    public interface ICommandProcessor
    {
        Task<string> ProcessAsync(string command);
    }
}
