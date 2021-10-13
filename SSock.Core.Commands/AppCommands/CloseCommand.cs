using SSock.Core.Commands.Abstract.AppCommands;

namespace SSock.Core.Commands.AppCommands
{
    internal sealed class CloseCommand
        : ICommand
    {
        public string Execute(string[] commandArgumants)
        {
            return "CLOSE";
        }
    }
}