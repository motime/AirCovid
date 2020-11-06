using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AirCovid.Api.Services.Commands;
using AirCovid.Api.Services.Commands.Validation;
using AirCovid.Data;
using AirCovid.Domain;
using FakeItEasy;
using Xunit;

namespace AirCovid.Tests
{
    public class CreateCheckInCommandValidatorTests
    {
        private readonly CreateCheckInCommandValidator _sut;

        private readonly ICheckInRepository _checkInRepository = A.Fake<ICheckInRepository>();
        private readonly IFlightRepository _flightRepository = A.Fake<IFlightRepository>();
        private readonly IPassengerRepository _passengerRepository = A.Fake<IPassengerRepository>();

        private readonly Flight _flight = new Flight()
        {
            FlightId = Guid.Empty,
            Policy = new Policy
            {
                AllowedBagsPerPassenger = 2,
                AllowedWeightPerPassenger = 5,
                AllowedSeats = 3,
                AllowedTotalWeight = 10
            }
        };

        public CreateCheckInCommandValidatorTests()
        {
            _sut = new CreateCheckInCommandValidator(_checkInRepository, _flightRepository, _passengerRepository);
        }

        [Fact]
        public async Task CheckIn_WhenFlightNotFound_ShouldCauseValidationError()
        {
            A.CallTo(() => _flightRepository.GetFlightById(A<Guid>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult((Flight) null));

            var validationResult = await _sut.Validate(new CreateCheckInCommand
            {
                CheckIn = new CheckIn
                {
                    FlightId = Guid.NewGuid(),
                    PassengerId = Guid.NewGuid(),
                    Bags = new List<Bag>()
                }
            }, CancellationToken.None);


            Assert.Equal(Problem.FlightNotFound, validationResult.Problem);
        }
        
        [Fact]
        public async Task CheckIn_WhenPassengerNotFound_ShouldCauseValidationError()
        {
            A.CallTo(() => _flightRepository.GetFlightById(_flight.FlightId, A<CancellationToken>.Ignored)).Returns(Task.FromResult(_flight));
            A.CallTo(() => _passengerRepository.Exists(A<Guid>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(false));

            var validationResult = await _sut.Validate(new CreateCheckInCommand
            {
                CheckIn = new CheckIn
                {
                    FlightId = _flight.FlightId,
                    PassengerId = Guid.NewGuid(),
                    Bags = new List<Bag>()
                }
            }, CancellationToken.None);


            Assert.Equal(Problem.PassengerDoesNotExist, validationResult.Problem);
        }

        [Fact]
        public async Task CheckIn_WhenPassengerHasTooManyBags_ShouldCauseValidationError()
        {
            A.CallTo(() => _flightRepository.GetFlightById(_flight.FlightId, A<CancellationToken>.Ignored)).Returns(Task.FromResult(_flight));
            A.CallTo(() => _passengerRepository.Exists(A<Guid>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));

            var validationResult = await _sut.Validate(new CreateCheckInCommand
            {
                CheckIn = new CheckIn
                {
                    FlightId = _flight.FlightId,
                    PassengerId = Guid.NewGuid(),
                    Bags = Enumerable.Range(1, 3).Select(i => new Bag()).ToList() //Too many. see _flight
                }
            }, CancellationToken.None);


            Assert.Equal(Problem.PassengerHasTooManyBags, validationResult.Problem);
        }
        
        [Fact]
        public async Task CheckIn_WhenPassengerBagsAreTooHeavy_ShouldCauseValidationError()
        {
            A.CallTo(() => _flightRepository.GetFlightById(_flight.FlightId, A<CancellationToken>.Ignored)).Returns(Task.FromResult(_flight));
            A.CallTo(() => _passengerRepository.Exists(A<Guid>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));


            var validationResult = await _sut.Validate(new CreateCheckInCommand
            {
                CheckIn = new CheckIn
                {
                    FlightId = _flight.FlightId,
                    PassengerId = Guid.NewGuid(),
                    Bags = new List<Bag>
                    {
                        new Bag
                        {
                            Weight = 3
                        },
                        new Bag
                        {
                            Weight = 2.1
                        }
                    }
                }
            }, CancellationToken.None);


            Assert.Equal(Problem.PassengersBagsAreTooHeavy, validationResult.Problem);
        }

        [Fact]
        public async Task CheckIn_WhenPassengerAlreadyCheckedIn_ShouldCauseValidationError()
        {
            Guid passengerId = Guid.NewGuid();

            IList<CheckIn> checkInListThatContainsThisPassenger = new List<CheckIn> {new CheckIn {PassengerId = passengerId}};


            A.CallTo(() => _flightRepository.GetFlightById(_flight.FlightId, A<CancellationToken>.Ignored)).Returns(Task.FromResult(_flight));
            A.CallTo(() => _passengerRepository.Exists(A<Guid>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => _checkInRepository.Get(A<Guid>.Ignored, CancellationToken.None)).Returns(Task.FromResult(checkInListThatContainsThisPassenger));


            var validationResult = await _sut.Validate(new CreateCheckInCommand
            {
                CheckIn = new CheckIn
                {
                    FlightId = _flight.FlightId,
                    PassengerId = passengerId,
                    Bags = Enumerable.Range(1, 1).Select(i => new Bag()).ToList()
                }
            }, CancellationToken.None);

            Assert.Equal(Problem.PassengerAlreadyCheckedIn, validationResult.Problem);
        }

        [Fact]
        public async Task CheckIn_WhenFlightExceededAllowedTotalWeight_ShouldCauseValidationError()
        {
            IList<CheckIn> currentCheckIns = new List<CheckIn>
            {
                new CheckIn
                {
                    PassengerId = Guid.NewGuid(),
                    Bags = new List<Bag>
                    {
                        new Bag {Weight = 9}
                    }
                }
            };

            A.CallTo(() => _flightRepository.GetFlightById(_flight.FlightId, A<CancellationToken>.Ignored)).Returns(Task.FromResult(_flight));
            A.CallTo(() => _passengerRepository.Exists(A<Guid>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => _checkInRepository.Get(A<Guid>.Ignored, CancellationToken.None)).Returns(Task.FromResult(currentCheckIns));

            var validationStatus = await _sut.Validate(new CreateCheckInCommand
            {
                CheckIn = new CheckIn
                {
                    FlightId = _flight.FlightId,
                    PassengerId = Guid.NewGuid(),
                    Bags = new List<Bag>
                    {
                        new Bag
                        {
                            Weight = 1.01
                        }
                    }
                }
            }, CancellationToken.None);

            Assert.Equal(Problem.ExceededFlightAllowedWeight, validationStatus.Problem);
        }

        [Fact]
        public async Task CheckIn_WhenFlightHasTooManyPassengers_ShouldCauseValidationError()
        {
            IList<CheckIn> currentCheckIns = Enumerable.Range(1, _flight.Policy.AllowedSeats)
                .Select(seat => new CheckIn
                {
                    PassengerId = Guid.NewGuid(),
                    Bags = new List<Bag>()
                    {
                        new Bag()
                        {
                            Weight = 0
                        }
                    },
                    FlightId = _flight.FlightId
                }).ToList();

            A.CallTo(() => _flightRepository.GetFlightById(_flight.FlightId, A<CancellationToken>.Ignored)).Returns(Task.FromResult(_flight));
            A.CallTo(() => _passengerRepository.Exists(A<Guid>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));
            A.CallTo(() => _checkInRepository.Get(A<Guid>.Ignored, CancellationToken.None)).Returns(Task.FromResult(currentCheckIns));

            var validationResult = await _sut.Validate(new CreateCheckInCommand
            {
                CheckIn = new CheckIn
                {
                    FlightId = _flight.FlightId,
                    PassengerId = Guid.NewGuid(),
                    Bags = new List<Bag>
                    {
                        new Bag
                        {
                            Weight = 0
                        }
                    }
                }
            }, CancellationToken.None);

            Assert.Equal(Problem.TooManyPassengersOnFlight, validationResult.Problem);
        }
    }
}