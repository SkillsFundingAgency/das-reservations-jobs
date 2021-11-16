using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ProviderPermissionRepository : IProviderPermissionRepository
    {
        private readonly IReservationsDataContext _dataContext;

        public ProviderPermissionRepository(IReservationsDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IEnumerable<ProviderPermission> GetAllWithCreateCohortPermission()
        {
            return _dataContext.ProviderPermissions
                .Where(c => c.CanCreateCohort);
        }

        public IEnumerable<ProviderPermission> GetAllForAccountLegalEntity(long accountLegalEntityId)
        {
            return _dataContext.ProviderPermissions
                .Where(permission => permission.AccountLegalEntityId == accountLegalEntityId)
                .Where(permission => permission.CanCreateCohort);
        }

        public async Task Add(ProviderPermission permission)
        {
            using (var transaction = new TransactionScope())
            {
                var existingPermission = await _dataContext.ProviderPermissions.FindAsync(permission.AccountId,
                    permission.AccountLegalEntityId, permission.ProviderId);

                if (existingPermission == null)
                {
                    await _dataContext.ProviderPermissions.AddAsync(permission);
                }
                else
                {
                    existingPermission.CanCreateCohort = permission.CanCreateCohort;

                }

                _dataContext.SaveChanges();
                transaction.Complete();
            }
        }
    }
}
