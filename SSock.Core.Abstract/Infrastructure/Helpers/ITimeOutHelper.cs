using System;
using System.Threading.Tasks;

namespace SSock.Core.Abstract.Infrastructure.Helpers
{
    public interface ITimeOutHelper
    {
        void StartTimeOutTracking(
            int timeout,
            Action onTimeOut);

        void StopTimeOutTracking();
    }
}
