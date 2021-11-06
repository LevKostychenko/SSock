using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Infrastructure.Extensions;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal sealed class EchoCommand 
        : ICommand
    {
        public async Task<string> ExecuteAsync(
            byte[] args,
            string clientId)
        {
            return await Task.Run(() => {
                if (args.Length > 0)
                {
                    return args.BytesToString();
                }

                return string.Empty;
            });
        }
    }
}
