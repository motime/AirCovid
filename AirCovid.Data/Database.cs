using System;
using System.Collections.Generic;
using AirCovid.Domain;

namespace AirCovid.Data
{
    public class Database
    {
        public static Dictionary<Guid, Flight> Flights = new Dictionary<Guid, Flight>()
        {
            {
                new Guid("55555555-5555-5555-5555-555555555555"), new Flight()
                {
                    FlightId = new Guid("55555555-5555-5555-5555-555555555555"),
                    Policy = new Policy()
                    {
                        AllowedBagsPerPassenger = 2,
                        AllowedTotalWeight = 25,
                        AllowedSeats = 5,
                        AllowedWeightPerPassenger = 5,
                    }
                }
            }
        };


        public static List<CheckIn> CheckIns = new List<CheckIn>();

        public static List<Passenger> Passengers = new List<Passenger>()
        {
            new Passenger {PassengerId = new Guid("00000000-0000-0000-0000-000000000010")},
            new Passenger {PassengerId = new Guid("00000000-0000-0000-0000-000000000020")}
        };
    }
}