namespace SSock.Core.Commands.Abstract
{
    public interface ICommand<TIn, TOut>
    {
        TOut Execute(TIn arguments);
    }
}
