using System.Threading.Tasks;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Application.ProviderPermissions.Handlers
{
    public class ProviderPermissionUpdatedHandler : IProviderPermissionUpdatedHandler
    {
        private readonly IProviderPermissionService _service;

        public ProviderPermissionUpdatedHandler(IProviderPermissionService service)
        {
            _service = service;
        }

        public async Task Handle(UpdatedPermissionsEvent updateEvent)
        {
            await _service.AddProviderPermission(updateEvent);
        }
    }
}
