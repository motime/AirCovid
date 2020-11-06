using System;
using System.Collections.Concurrent;
using System.Timers;
using Nito.AsyncEx;

namespace AirCovid.Api.Infra
{
    /// <summary>
    /// A factory for creating AsyncLocks that expire afters some time of not being accessed
    /// Currently expiry time is hard code, but it can be changed easily
    /// </summary>
    public class TimedAsyncLockFactory : ITimedAsyncLockFactory, IDisposable
    { 
        public class LockManager
        {
            public AsyncLock Lock { get; set; } = new AsyncLock();
            public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
        }

        private readonly Timer _timer;

        public TimedAsyncLockFactory()
        {
            double every30Minutes = 30 * 60 * 1000;
            _timer = new Timer(every30Minutes);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var lockManager in _lockManagers)
            {
                var flightAccessedRecently = lockManager.Value.LastAccessed >= DateTime.UtcNow.AddHours(-2);
                if (flightAccessedRecently) 
                    continue;

                _lockManagers.TryRemove(lockManager.Key, out _);
            }
        }
        
        private readonly ConcurrentDictionary<Guid, LockManager> _lockManagers = new ConcurrentDictionary<Guid, LockManager>();

        public AsyncLock AcquireLock(Guid lockId)
        {
            
            var lockManager = _lockManagers.GetOrAdd(lockId, (guid) => new LockManager());
            lockManager.LastAccessed = DateTime.UtcNow;
            return lockManager.Lock;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}