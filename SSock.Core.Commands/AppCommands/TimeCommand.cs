using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Infrastructure.Extensions;
using System;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal sealed class TimeCommand
        : ICommand
    {
        private const string UtcArg = "-utc";

        public async Task<string> ExecuteAsync(
            byte[] args,
            string clientId)
        {
            return await Task.Run(() =>
            {
                if (args.Length > 0)
                {
                    var commandArgs = args.BytesToString();

                    if (commandArgs == UtcArg)
                    {
                        return DateTime.UtcNow.ToString();
                    }
                }

                return DateTime.Now.ToString();
            });
        }
    }
}
