using System;
using System.Collections.Concurrent;
using AirCovid.Domain;

namespace AirCovid.Data
{
    public class Database
    {
        public static ConcurrentBag<CheckIn> CheckIns = new ConcurrentBag<CheckIn>();

        public static ConcurrentBag<Flight> Flights = new ConcurrentBag<Flight>
        {
            new Flight
            {
                FlightId = new Guid("55555555-5555-5555-5555-555555555555"),
                Policy = new Policy()
                {
                    AllowedBagsPerPassenger = 2,
                    AllowedTotalWeight = 25,
                    AllowedSeats = 5,
                    AllowedWeightPerPassenger = 5,
                }
            },

            new Flight
            {
                FlightId = new Guid("66666666-6666-6666-6666-666666666666"),
                Policy = new Policy()
                {
                    AllowedBagsPerPassenger = 2,
                    AllowedTotalWeight = 25,
                    AllowedSeats = 5,
                    AllowedWeightPerPassenger = 5,
                }
            }
        };

        public static ConcurrentBag<Passenger> Passengers = new ConcurrentBag<Passenger>()
        {
            new Passenger {PassengerId = new Guid("00000000-0000-0000-0000-000000000010")},
            new Passenger {PassengerId = new Guid("00000000-0000-0000-0000-000000000020")}
        };
    }
}