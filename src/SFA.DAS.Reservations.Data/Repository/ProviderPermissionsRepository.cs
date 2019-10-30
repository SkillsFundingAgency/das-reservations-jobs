using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ProviderPermissionsRepository : IProviderPermissionsRepository
    {
        private readonly IReservationsDataContext _context;

        public ProviderPermissionsRepository(IReservationsDataContext context)
        {
            _context = context;
        }

        public IEnumerable<ProviderPermission> GetAllWithCreateCohortPermission()
        {
            return _context.ProviderPermissions.Where(c=>c.CanCreateCohort).ToArray();
        }
    }
}
