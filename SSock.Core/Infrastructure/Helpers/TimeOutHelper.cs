using SSock.Core.Abstract.Infrastructure.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SSock.Core.Infrastructure.Helpers
{
    internal class TimeOutHelper
        : ITimeOutHelper
    {
        private bool _isStopCalled;

        public async Task StartTimeOutTrackingAsync(
            int timeout,
            Action onTimeOut)
            =>  await Task.Factory.StartNew(() =>
                {
                    _isStopCalled = false;
                    Thread.Sleep(timeout);
                
                    if (!_isStopCalled)
                    {
                        onTimeOut();
                    }
                });
        
        public void StopTimeOutTracking()
            => _isStopCalled = true;        
    }
}
