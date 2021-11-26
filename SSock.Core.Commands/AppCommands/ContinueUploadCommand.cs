using SSock.Core.Commands.Abstract.AppCommands;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal class ContinueUploadCommand
        : ICommand
    {
        public Task<object> ExecuteAsync(byte[] tail, byte[] args, string clientId)
        {
            throw new System.NotImplementedException();
        }
    }
}
