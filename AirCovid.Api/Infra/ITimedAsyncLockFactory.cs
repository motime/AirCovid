using System;
using Nito.AsyncEx;

namespace AirCovid.Api.Infra
{
    public interface ITimedAsyncLockFactory
    {
        AsyncLock AcquireLock(Guid lockId);
    }
}