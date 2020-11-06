using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AirCovid.Domain;

namespace AirCovid.Data
{
    public class CheckInRepository : ICheckInRepository
    {
        public Task Add(CheckIn checkIn, CancellationToken cancellationToken)
        {
            Database.CheckIns.Add(checkIn);
            return Task.CompletedTask;
        }

        public Task<IList<CheckIn>> Get(Guid flightId, CancellationToken cancellationToken)
        {
            var checkIns = Database.CheckIns.Where(chkIn => chkIn.FlightId == flightId).ToList();
            return Task.FromResult<IList<CheckIn>>(checkIns);
        }
    }
}