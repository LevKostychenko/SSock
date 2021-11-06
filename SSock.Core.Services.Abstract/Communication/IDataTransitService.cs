namespace SSock.Core.Services.Abstract.Communication
{
    public interface IDataTransitService
    {
        byte[] AppendBytes(byte[] initialBytes, int count);
    }
}
