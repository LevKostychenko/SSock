namespace SSock.Core.Abstract.Infrastructure.MessageObserver
{
    public interface IObservable
    {
        void RegisterObserver(IObserver observer);

        void RemoveObserver(IObserver observer);

        void NotifyObservers();
    }
}
