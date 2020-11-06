using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AirCovid.Data
{
    public class PassengerRepository : IPassengerRepository
    {
        public Task<bool> Exists(Guid passengerId, CancellationToken cancellationToken)
        {
            var exists = Database.Passengers.Any(p => p.PassengerId == passengerId);
            return Task.FromResult(exists);
        }
    }
}