using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers
{
    public class AddAccountLegalEntityHandler : IAddAccountLegalEntityHandler
    {
        private readonly IAccountLegalEntitiesService _service;
        private readonly IValidator<AddedLegalEntityEvent> _validator;
        private readonly ILogger<AddAccountLegalEntityHandler> _logger;

        public AddAccountLegalEntityHandler(
            IAccountLegalEntitiesService service, 
            IValidator<AddedLegalEntityEvent> validator, 
            ILogger<AddAccountLegalEntityHandler> logger)
        {
            _service = service;
            _validator = validator;
            _logger = logger;
        }

        public async Task Handle(AddedLegalEntityEvent accountLegalEntityAddedEvent)
        {
            var validationResult = await _validator.ValidateAsync(accountLegalEntityAddedEvent);
            if (validationResult.IsValid())
            {
                await _service.AddAccountLegalEntity(accountLegalEntityAddedEvent);
            }
            else
            {
                _logger.LogWarning($"Could not add legal entity to database because it is invalid for the following reasons: {validationResult}");
            }
        }
    }
}
