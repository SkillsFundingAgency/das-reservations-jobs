using System;
using System.Threading.Tasks;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Application.ProviderPermissions.Handlers
{
    public class ProviderPermissionUpdatedHandler : IProviderPermissionUpdatedHandler
    {
        public Task Handle(UpdatedPermissionsEvent updateEvent)
        {
            throw new NotImplementedException();
        }
    }
}
