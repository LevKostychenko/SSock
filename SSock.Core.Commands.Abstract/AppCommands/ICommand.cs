using System.Threading.Tasks;

namespace SSock.Core.Commands.Abstract.AppCommands
{
    public interface ICommand
    {
        Task<object> ExecuteAsync(
            byte[] tail,
            byte[] args,
            string clientId);
    }
}
