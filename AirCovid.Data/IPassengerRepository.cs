using System;
using System.Threading;
using System.Threading.Tasks;

namespace AirCovid.Data
{
    public interface IPassengerRepository
    {
        Task<bool> Exists(Guid passengerId, CancellationToken cancellationToken);
    }
}