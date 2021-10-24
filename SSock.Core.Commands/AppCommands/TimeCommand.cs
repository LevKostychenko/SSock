using SSock.Core.Commands.Abstract.AppCommands;
using System;
using System.Threading.Tasks;

namespace SSock.Core.Commands.AppCommands
{
    internal sealed class TimeCommand
        : ICommand
    {
        private const string UtcArg = "-utc";

        public async Task<string> ExecuteAsync(
            string[] commandArgumants,
            string clientId)
        {
            return await Task.Run(() =>
            {
                if (commandArgumants.Length > 0)
                {
                    if (commandArgumants[0] == UtcArg)
                    {
                        return DateTime.UtcNow.ToString();
                    }
                }

                return DateTime.Now.ToString();
            });
        }
    }
}
