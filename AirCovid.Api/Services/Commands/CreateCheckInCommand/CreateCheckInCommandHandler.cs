using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
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
        private readonly ICreateCheckInCommandValidator _validator;
        private readonly ICheckInRepository _checkInRepository;

        private readonly ConcurrentDictionary<Guid, AsyncLock> _locks = new ConcurrentDictionary<Guid, AsyncLock>();

        public CreateCheckInCommandHandler(ICreateCheckInCommandValidator validator, ICheckInRepository checkInRepository)
        {
            _validator = validator;
            _checkInRepository = checkInRepository;
        }

        public async Task<CreateCheckInsResponse> Handle(CreateCheckInCommand request, CancellationToken cancellationToken)
        {
            //We will get a lock per flight id. 
            var @lock = _locks.GetOrAdd(request.CheckIn.FlightId, (guid) => new AsyncLock());

            using (await @lock.LockAsync(cancellationToken))
            {
                using (new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var validationResult = await _validator.Validate(request, cancellationToken);
                    if (validationResult.Passed == false)
                        return new CreateCheckInsResponse(validationResult);

                    await _checkInRepository.Add(request.CheckIn, cancellationToken);
                    return new CreateCheckInsResponse(request.CheckIn.CheckInId);
                }
            }
        }
    }
}