using SSock.Core.Commands.Abstract.AppCommands;
using System;

namespace SSock.Core.Commands.AppCommands
{
    internal sealed class TimeCommand
        : ICommand
    {
        private const string UtcArg = "-utc";

        public string Execute(string[] commandArgumants)
        {
            if (commandArgumants.Length > 0)
            {
                if (commandArgumants[0] == UtcArg)
                {
                    return DateTime.UtcNow.ToString();
                }
            }

            return DateTime.Now.ToString();
        }
    }
}
