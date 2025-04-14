using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Application.ProviderPermissions.Handlers
{
    public class ProviderPermissionsUpdatedHandler(
        IUpdatedPermissionsEventValidator validator,
        IProviderPermissionService service,
        ILogger<ProviderPermissionsUpdatedHandler> logger)
        : IProviderPermissionsUpdatedHandler
    {
        public async Task Handle(UpdatedPermissionsEvent updateEvent)
        {
            if (!validator.Validate(updateEvent))
            {
                logger.LogWarning(
                    $"Cannot process provider permission event due to validation failure " +
                    $"Account ID: {updateEvent?.AccountId}, Ukprn: {updateEvent?.Ukprn}, Account legal entity ID:{updateEvent?.AccountLegalEntityId}");
                return;
            }

            await service.AddProviderPermission(updateEvent);
        }
    }
}
