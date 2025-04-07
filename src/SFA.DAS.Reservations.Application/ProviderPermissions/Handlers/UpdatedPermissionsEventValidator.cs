using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Application.ProviderPermissions.Handlers
{
    public class UpdatedPermissionsEventValidator(ILogger<UpdatedPermissionsEventValidator> logger)
        : IUpdatedPermissionsEventValidator
    {
        public bool Validate(UpdatedPermissionsEvent updatedPermissionsEvent)
        {
            if (updatedPermissionsEvent.AccountId.Equals(default))
            {
                logger.LogInformation($"Validation failure: {nameof(updatedPermissionsEvent.AccountId)} must be set in event");
                return false;
            }

            if (updatedPermissionsEvent.AccountLegalEntityId.Equals(default))
            {
                logger.LogInformation($"Validation failure: {nameof(updatedPermissionsEvent.AccountLegalEntityId)} must be set in event");
                return false;
            }

            if (updatedPermissionsEvent.Ukprn.Equals(default))
            {
                logger.LogInformation($"Validation failure: {nameof(updatedPermissionsEvent.Ukprn)} must be set in event");
                return false;
            }

            return true;
        }
    }
}