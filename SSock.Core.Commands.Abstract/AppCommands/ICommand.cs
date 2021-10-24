using System.Threading.Tasks;

namespace SSock.Core.Commands.Abstract.AppCommands
{
    public interface ICommand
    {
        Task<string> ExecuteAsync(
            string[] commandArgumants,
            string clientId);
    }
}
