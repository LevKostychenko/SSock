namespace SSock.Core.Commands.Abstract.AppCommands
{
    public interface ICommand
    {
        string Execute(string[] commandArgumants);
    }
}
