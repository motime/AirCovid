using System;
using AirCovid.Api.Services.Commands.Validation;

namespace AirCovid.Api.Services.Commands
{
    public class CreateCheckInsResponse
    {
        public ValidationResult ValidationResult { get; set; } = ValidationResult.NoProblem;
        public Guid? CheckInId { get; set; }
    }
}