using SSock.Core.Commands.Abstract.AppCommands;

namespace SSock.Core.Commands.AppCommands
{
    internal sealed class EchoCommand 
        : ICommand
    {
        public string Execute(string[] commandArgumants)
        {
            if (commandArgumants.Length > 0)
            {
                return string.Join(" ", commandArgumants);
            }

            return string.Empty;
        }
    }
}
