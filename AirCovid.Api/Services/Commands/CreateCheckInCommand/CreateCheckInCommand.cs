using AirCovid.Domain;
using MediatR;

namespace AirCovid.Api.Services.Commands
{
    public class CreateCheckInCommand : IRequest<CreateCheckInsResponse>
    {
        public CheckIn CheckIn { get; set; }
    }
}