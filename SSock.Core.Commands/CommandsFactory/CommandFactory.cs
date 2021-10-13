using SSock.Core.Commands.Abstract.AppCommands;
using SSock.Core.Commands.Abstract.CommandsFactory;
using SSock.Core.Commands.AppCommands;

namespace SSock.Core.Commands.CommandsFactory
{
    internal class CommandFactory
        : ICommandFactory
    {
        public ICommand CreateCommand(string command)
        {
            switch (command)
            {
                case CommandsNames.CloseCommand:
                    {
                        return new CloseCommand();
                    }
                case CommandsNames.EchoCommand:
                    {
                        return new EchoCommand();
                    }
                case CommandsNames.TimeCommand:
                    {
                        return new TimeCommand();
                    }
                case CommandsNames.UploadCommand:
                    {
                        return new UploadFileCommand();
                    }
                default:
                    {
                        return default;
                    }
            }
        }
    }
}
