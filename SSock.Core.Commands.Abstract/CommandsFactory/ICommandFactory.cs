using SSock.Core.Commands.Abstract.AppCommands;

namespace SSock.Core.Commands.Abstract.CommandsFactory
{
    public interface ICommandFactory
    {
        ICommand CreateCommand(string command);
    }
}
