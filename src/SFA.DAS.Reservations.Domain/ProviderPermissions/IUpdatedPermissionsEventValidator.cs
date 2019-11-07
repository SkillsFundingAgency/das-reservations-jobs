using SFA.DAS.ProviderRelationships.Messages.Events;

namespace SFA.DAS.Reservations.Domain.ProviderPermissions
{
    public interface IUpdatedPermissionsEventValidator
    {
        bool Validate(UpdatedPermissionsEvent updatedPermissionsEvent);
    }
}