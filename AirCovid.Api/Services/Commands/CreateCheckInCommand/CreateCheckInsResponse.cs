using System;
using AirCovid.Api.Services.Commands.Validation;

namespace AirCovid.Api.Services.Commands
{
    public class CreateCheckInsResponse
    {
        public CreateCheckInsResponse(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }
        public CreateCheckInsResponse(Guid checkInId)
        {
            CheckInId = CheckInId;
        }


        public ValidationResult ValidationResult { get; } = new ValidationResult(true);
        public Guid? CheckInId { get; }
    }
}