using System;
using System.Threading.Tasks;

namespace SSock.Core.Abstract.Infrastructure.Helpers
{
    public interface ITimeOutHelper
    {
        Task StartTimeOutTrackingAsync(
            int timeout,
            Action onTimeOut);

        void StopTimeOutTracking();
    }
}
