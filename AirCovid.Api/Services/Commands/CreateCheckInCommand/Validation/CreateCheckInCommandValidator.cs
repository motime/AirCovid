using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AirCovid.Data;
using AirCovid.Domain;

namespace AirCovid.Api.Services.Commands.Validation
{
    public class CreateCheckInCommandValidator : ICreateCheckInCommandValidator
    {
        private readonly ICheckInRepository _checkInRepository;
        private readonly IPassengerRepository _passengerRepository;
        private readonly IFlightRepository _flightRepository;

        public CreateCheckInCommandValidator(ICheckInRepository checkInRepository,
            IFlightRepository flightRepository,
            IPassengerRepository passengerRepository)
        {
            _flightRepository = flightRepository;
            _passengerRepository = passengerRepository;
            _checkInRepository = checkInRepository;
        }

        public async Task<ValidationResult> Validate(CreateCheckInCommand request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult;

            var flight = await _flightRepository.GetFlightById(request.CheckIn.FlightId, cancellationToken);
            if (flight == null)
                return new ValidationResult(false, Problem.FlightNotFound, $"flight {request.CheckIn.FlightId} does not exists");

            bool passengerExists = await _passengerRepository.Exists(request.CheckIn.PassengerId, cancellationToken);
            if (passengerExists == false)
                return new ValidationResult(false, Problem.PassengerDoesNotExist, $"passenger {request.CheckIn.PassengerId} does not exists");

            if (PassengerHasTooManyBags(request, flight, out validationResult))
                return validationResult;

            if (PassengerBagsAreTooHeavy(request, flight, out validationResult))
                return validationResult;

            var checkIns = await _checkInRepository.Get(flight.FlightId, cancellationToken);

            if (PassengerAlreadyCheckedIn(request, checkIns, out validationResult))
                return validationResult;
            ;
            if (ExceededFlightAllowedWeight(request, checkIns, flight, out validationResult))
                return validationResult;

            if (TooManyPassengersOnFlight(checkIns, flight, out validationResult))
                return validationResult;

            return new ValidationResult(true);
        }

        private static bool PassengerBagsAreTooHeavy(CreateCheckInCommand request, Flight flight, out ValidationResult validationResult)
        {
            var bagsAreTooHeavy = request.CheckIn.Bags.Sum(b => b.Weight) > flight.Policy.AllowedWeightPerPassenger;
            if (bagsAreTooHeavy == false)
            {
                validationResult = ValidationResult.NoProblem;
                return false;
            }

            string message = $"passenger bags exceed limit of {flight.Policy.AllowedWeightPerPassenger} KG";
            validationResult = new ValidationResult(false, Problem.PassengersBagsAreTooHeavy, message);
            return true;
        }

        private static bool PassengerHasTooManyBags(CreateCheckInCommand request, Flight flight, out ValidationResult validationResult)
        {
            var tooManyBags = request.CheckIn.Bags.Count > flight.Policy.AllowedBagsPerPassenger;
            if (tooManyBags == false)
            {
                validationResult = ValidationResult.NoProblem;
                return false;
            }

            string message = $"passenger is allowed only {flight.Policy.AllowedBagsPerPassenger} bags";
            validationResult = new ValidationResult(false, Problem.PassengerHasTooManyBags, message);
            return true;
        }

        private static bool PassengerAlreadyCheckedIn(CreateCheckInCommand request, IList<CheckIn> checkIns, out ValidationResult validationResult)
        {
            var isPassengerAlreadyCheckedIn = checkIns.Any(c => c.PassengerId == request.CheckIn.PassengerId);
            if (isPassengerAlreadyCheckedIn == false)
            {
                validationResult = ValidationResult.NoProblem;
                return false;
            }

            string message = "passenger is already checkedIn to flight";
            validationResult = new ValidationResult(false, Problem.PassengerAlreadyCheckedIn, message);
            return true;
        }

        private static bool ExceededFlightAllowedWeight(CreateCheckInCommand request, IList<CheckIn> checkIns, Flight flight, out ValidationResult validationResult)
        {
            var currentFlightWeight = checkIns.SelectMany(chkIn => chkIn.Bags).Sum(c => c.Weight);
            var passengerFlightWeight = request.CheckIn.Bags.Sum(bag => bag.Weight);
            bool exceededFlightAllowedWeight = currentFlightWeight + passengerFlightWeight > flight.Policy.AllowedTotalWeight;

            if (exceededFlightAllowedWeight == false)
            {
                validationResult = ValidationResult.NoProblem;
                return false;
            }

            string message = $"Checking in passenger bags will make the baggage weight to exceed maximum allowed weight of {flight.Policy.AllowedTotalWeight} KG";
            validationResult = new ValidationResult(false, Problem.ExceededFlightAllowedWeight, message);
            return true;
        }

        private static bool TooManyPassengersOnFlight(ICollection<CheckIn> checkIns, Flight flight, out ValidationResult validationResult)
        {
            bool tooManyPassengersOnFlight = checkIns.Count + 1 > flight.Policy.AllowedSeats;
            if (tooManyPassengersOnFlight == false)
            {
                validationResult = ValidationResult.NoProblem;
                return false;
            }

            string message = $"Plane reached the maximum capacity of {flight.Policy.AllowedSeats} passengers";
            validationResult = new ValidationResult(false, Problem.TooManyPassengersOnFlight, message);
            return true;
        }
    }
}