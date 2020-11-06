using System;

namespace AirCovid.Domain
{
    public class Flight
    {
        public Guid FlightId { get; set; }

        public Policy Policy { get; set; }
    }
}