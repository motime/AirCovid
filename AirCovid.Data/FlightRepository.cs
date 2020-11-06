using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AirCovid.Domain;

namespace AirCovid.Data
{
    public class FlightRepository : IFlightRepository
    {
        public Task<Flight> GetFlightById(Guid flightId, CancellationToken token)
        {
            var flight = Database.Flights.SingleOrDefault(f => f.FlightId == flightId);
            return Task.FromResult(flight);
        }
    }
}