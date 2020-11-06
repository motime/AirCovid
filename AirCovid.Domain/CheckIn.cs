using System;
using System.Collections.Generic;

namespace AirCovid.Domain
{
    public class CheckIn
    {
        public Guid CheckInId { get; set; }

        public Guid FlightId { get; set; }

        public Guid PassengerId { get; set; }

        public IList<Bag> Bags { get; set; }
    }
}