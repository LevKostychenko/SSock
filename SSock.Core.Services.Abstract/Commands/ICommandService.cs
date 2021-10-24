namespace SSock.Core.Services.Abstract.Commands
{
    public interface ICommandService
    {
        (string command, string[] args) ParseCommand(string command);
    }
}
