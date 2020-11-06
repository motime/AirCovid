using System;
using System.Threading;
using System.Threading.Tasks;
using AirCovid.Domain;

namespace AirCovid.Data
{
    public class FlightRepository : IFlightRepository
    {
        public Task<Flight> GetFlightById(Guid flightId, CancellationToken token)
        {
            return Database.Flights.TryGetValue(flightId, out var flight) ? 
                Task.FromResult(flight) :
                Task.FromResult((Flight) null);
        }
    }
}