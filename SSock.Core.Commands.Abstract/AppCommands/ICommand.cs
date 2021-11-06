using System.Threading.Tasks;

namespace SSock.Core.Commands.Abstract.AppCommands
{
    public interface ICommand
    {
        Task<string> ExecuteAsync(
            byte[] args, 
            string clientId);
    }
}
