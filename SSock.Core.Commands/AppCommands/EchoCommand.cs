using SSock.Core.Commands.Abstract.AppCommands;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal sealed class EchoCommand 
        : ICommand
    {
        public async Task<string> ExecuteAsync(
            string[] commandArgumants,
            string clientId)
        {
            return await Task.Run(() => { 
                if (commandArgumants.Length > 0)
                {
                    return string.Join(" ", commandArgumants);
                }

                return string.Empty;
            });
        }
    }
}
