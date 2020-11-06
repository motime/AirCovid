using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AirCovid.Api.Infra;
using AirCovid.Api.Services.Commands.Validation;
using AirCovid.Data;
using MediatR;
using Nito.AsyncEx;

namespace AirCovid.Api.Services.Commands
{
    /// <summary>
    /// Handler for CreateCheckInCommand
    /// </summary>
    internal class CreateCheckInCommandHandler : IRequestHandler<CreateCheckInCommand, CreateCheckInsResponse>
    {
        private readonly ITimedAsyncLockFactory _lockFactory;
        private readonly ICreateCheckInCommandValidator _validator;
        private readonly ICheckInRepository _checkInRepository;


        public CreateCheckInCommandHandler(ICreateCheckInCommandValidator validator, ICheckInRepository checkInRepository, ITimedAsyncLockFactory lockFactory)
        {
            _lockFactory = lockFactory;
            _validator = validator;
            _checkInRepository = checkInRepository;
        }

        public async Task<CreateCheckInsResponse> Handle(CreateCheckInCommand request, CancellationToken cancellationToken)
        {
            //We will get a lock per flight id. 
            var asyncLock = _lockFactory.AcquireLock(request.CheckIn.FlightId);

            using (await asyncLock.LockAsync(cancellationToken))
            {
                using (new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var validationResult = await _validator.Validate(request, cancellationToken);
                    if (validationResult.Passed == false)
                        return new CreateCheckInsResponse { ValidationResult = validationResult };

                    await _checkInRepository.Add(request.CheckIn, cancellationToken);

                    var response = new CreateCheckInsResponse { CheckInId = request.CheckIn.CheckInId };
                    return response;
                }
            }
        }
    }
}