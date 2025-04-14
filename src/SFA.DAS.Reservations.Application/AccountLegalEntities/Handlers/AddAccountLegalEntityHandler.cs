using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers
{
    public class AddAccountLegalEntityHandler(
        IAccountLegalEntitiesService service,
        IValidator<AddedLegalEntityEvent> validator,
        ILogger<AddAccountLegalEntityHandler> logger)
        : IAddAccountLegalEntityHandler
    {
        public async Task Handle(AddedLegalEntityEvent accountLegalEntityAddedEvent)
        {
            var validationResult = await validator.ValidateAsync(accountLegalEntityAddedEvent);
            if (validationResult.IsValid())
            {
                await service.AddAccountLegalEntity(accountLegalEntityAddedEvent);
            }
            else
            {
                logger.LogWarning($"Could not add legal entity to database because it is invalid for the following reasons: {validationResult}");
            }
        }
    }
}
