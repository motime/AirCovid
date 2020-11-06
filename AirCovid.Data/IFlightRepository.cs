using System;
using System.Threading;
using System.Threading.Tasks;
using AirCovid.Domain;

namespace AirCovid.Data
{
    public interface IFlightRepository
    {
        Task<Flight> GetFlightById(Guid flightId, CancellationToken token);
    }
}