using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.ProviderPermissions
{
    public interface IProviderPermissionRepository
    {
        Task Add(Entities.ProviderPermission permission);
    }
}
