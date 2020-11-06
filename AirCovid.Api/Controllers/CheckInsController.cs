using System;
using System.Threading.Tasks;
using AirCovid.Api.Models;
using AirCovid.Api.Models.SwaggerExamples;
using AirCovid.Api.Services.Commands;
using AirCovid.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Filters;

namespace AirCovid.Api.Controllers
{
    [ApiController]
    public class CheckInsController : ControllerBase
    {
        private readonly ILogger<CheckInsController> _logger;
        private readonly IMediator _mediator;

        public CheckInsController(ILogger<CheckInsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// </summary>
        /// <param name="flightId">Use: 55555555-5555-5555-5555-555555555555</param>
        /// <param name="createModel">model supplied in tests refers to existing passenger</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/flights/{flightId:guid}/checkins/")]
        [SwaggerRequestExample(typeof(CreateCheckInModel), typeof(CreateCheckInModelExample))]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Guid), 200)]
        public async Task<IActionResult> PostCheckIn([FromRoute] Guid flightId, [FromBody] CreateCheckInModel createModel)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"Posts checking with parameters of {createModel}");

            //This is performed her for clarity of code. In a real application we will use something like automapper.
            var checkIn = new CheckIn
            {
                PassengerId = createModel.PassengerId ?? throw new InvalidOperationException("mapping error occured"),
                Bags = createModel.Bags,
                FlightId = flightId,
                CheckInId = Guid.NewGuid()
            };

            var createResponse = await _mediator.Send(new CreateCheckInCommand {CheckIn = checkIn});

            if (_logger.IsEnabled(LogLevel.Debug) && createResponse.ValidationResult.Passed == false)
            {
                _logger.LogDebug($"Validation failed: {createResponse.ValidationResult.Problem} - {createResponse.ValidationResult.Message}");
            }

            var result = createResponse.ValidationResult.Passed ? Ok(createResponse.CheckInId) : CreateProblemDetails(createResponse);
            return result;
        }

        [NonAction]
        private IActionResult CreateProblemDetails(CreateCheckInsResponse createResponse)
        {
            switch (createResponse.ValidationResult.Problem)
            {
                case Services.Commands.Validation.Problem.FlightNotFound:
                    return Problem(
                        title: createResponse.ValidationResult.Problem.ToString(),
                        detail: Services.Commands.Validation.Problem.PassengerHasTooManyBags.ToString(),
                        instance: Request.GetDisplayUrl(),
                        statusCode: StatusCodes.Status404NotFound);

                default:
                    return Problem(
                        title: createResponse.ValidationResult.Problem.ToString(),
                        detail: createResponse.ValidationResult.Message,
                        instance: Request.GetDisplayUrl(),
                        statusCode: StatusCodes.Status422UnprocessableEntity);
            }
        }
    }
}