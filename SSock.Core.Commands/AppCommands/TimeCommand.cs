using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Infrastructure.Extensions;
using SSock.Core.Services.Abstract.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal sealed class TimeCommand
        : ICommand
    {
        private const string UtcArg = "-utc";

        private readonly IDataTransitService _dataTransitService;

        public TimeCommand(IDataTransitService dataTransitService)
        {
            _dataTransitService = dataTransitService;
        }

        public async Task<object> ExecuteAsync(
            byte[] tail,
            byte[] args,
            string clientId)
        {
            return await Task.Run(() =>
            {
                if (args.Length > 0)
                {
                    var commandArgs = _dataTransitService.ConvertFromByteArray<List<string>>(
                        args,
                        args.Length);

                    if (commandArgs.First() == UtcArg)
                    {
                        return DateTime.UtcNow.ToString();
                    }
                }

                return DateTime.Now.ToString();
            });
        }
    }
}
