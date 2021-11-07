using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Infrastructure.Extensions;
using SSock.Core.Services.Abstract.Communication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal sealed class EchoCommand 
        : ICommand
    {
        private readonly IDataTransitService _dataTransitService;

        public EchoCommand(IDataTransitService dataTransitService)
        {
            _dataTransitService = dataTransitService;
        }

        public async Task<string> ExecuteAsync(
            byte[] args,
            string clientId)
        {
            return await Task.Run(() => {
                if (args.Length > 0)
                {
                    var commandArgs = _dataTransitService.ConvertFromByteArray<List<string>>(
                        args,
                        args.Length);

                    return string.Join(' ', commandArgs);
                }

                return string.Empty;
            });
        }
    }
}
