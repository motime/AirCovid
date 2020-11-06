namespace AirCovid.Api.Services.Commands.Validation
{
    public enum Problem
    {
        Undefined,
        FlightNotFound,
        PassengerHasTooManyBags,
        PassengerAlreadyCheckedIn,
        ExceededFlightAllowedWeight,
        TooManyPassengersOnFlight,
        PassengerDoesNotExist,
        PassengersBagsAreTooHeavy
    }
}
