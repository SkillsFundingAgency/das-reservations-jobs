using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Domain.ProviderPermissions
{
    public interface IProviderPermissionsRepository
    {
        IEnumerable<ProviderPermission> GetAllWithCreateCohortPermission();
    }
}
