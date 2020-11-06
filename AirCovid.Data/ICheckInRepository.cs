using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AirCovid.Domain;

namespace AirCovid.Data
{
    public interface ICheckInRepository
    {
        Task Add(CheckIn checkIn, CancellationToken cancellationToken);

        Task<IList<CheckIn>> Get(Guid flightId, CancellationToken cancellationToken);
    }
}