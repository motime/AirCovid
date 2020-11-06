using System.Threading;
using System.Threading.Tasks;

namespace AirCovid.Api.Services.Commands.Validation
{
    internal interface ICreateCheckInCommandValidator
    {
        Task<ValidationResult> Validate(CreateCheckInCommand command, CancellationToken cancellationToken);
    }
}