using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Application.ProviderPermissions.Handlers
{
    public class ProviderPermissionsUpdatedHandler : IProviderPermissionsUpdatedHandler
    {
        private readonly IUpdatedPermissionsEventValidator _validator;
        private readonly IProviderPermissionService _service;
        private readonly ILogger<ProviderPermissionsUpdatedHandler> _logger;

        public ProviderPermissionsUpdatedHandler(
            IUpdatedPermissionsEventValidator validator,
            IProviderPermissionService service,
            ILogger<ProviderPermissionsUpdatedHandler> logger)
        {
            _validator = validator;
            _service = service;
            _logger = logger;
        }

        public async Task Handle(UpdatedPermissionsEvent updateEvent)
        {
            _logger.LogWarning("Processing UpdatedPermissionsEventfor Account ID {0}, Ukprn: {1}, Account legal entity ID:{2}", updateEvent?.AccountId, updateEvent?.Ukprn, updateEvent?.AccountLegalEntityId);

            if (!_validator.Validate(updateEvent))
            {
                _logger.LogWarning(
                    $"Cannot process provider permission event due to validation failure " +
                    $"Account ID: {updateEvent?.AccountId}, Ukprn: {updateEvent?.Ukprn}, Account legal entity ID:{updateEvent?.AccountLegalEntityId}");
                return;
            }

            await _service.AddProviderPermission(updateEvent);
        }
    }
}
