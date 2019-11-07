using System.Threading.Tasks;
using SFA.DAS.ProviderRelationships.Messages.Events;

namespace SFA.DAS.Reservations.Domain.ProviderPermissions
{
    public interface IProviderPermissionUpdatedHandler
    {
        Task Handle(UpdatedPermissionsEvent updateEvent);
    }
}
