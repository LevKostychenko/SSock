using SSock.Core.Commands.Abstract.AppCommands;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal sealed class CloseCommand
        : ICommand
    {
        public async Task<string> ExecuteAsync(
            byte[] args,
            string clientId)
        {
            return await Task.Run(() => { return "CLOSE"; });
        }
    }
}