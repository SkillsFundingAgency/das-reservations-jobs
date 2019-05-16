using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class AccountLegalEntityRepository
    {
        private readonly IReservationsDataContext _dataContext;

        public AccountLegalEntityRepository(IReservationsDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task Add(AccountLegalEntity accountLegalEntity)
        {
            using (var transaction = _dataContext.Database.BeginTransaction())
            {
                await _dataContext.AccountLegalEntities.AddAsync(accountLegalEntity);
                _dataContext.SaveChanges();
                transaction.Commit();
            }
        }

        public async Task UpdateAgreementStatus(AccountLegalEntity accountLegalEntity)
        {
            using (var transaction = _dataContext.Database.BeginTransaction())
            {
                var entity = await _dataContext.AccountLegalEntities.SingleOrDefaultAsync(c =>
                    c.AccountLegalEntityId.Equals(accountLegalEntity.AccountLegalEntityId));

                if (entity != null)
                {
                    entity.AgreementSigned = true;
                    _dataContext.SaveChanges();
                }

                transaction.Commit();
            }
        }

        public async Task Remove(AccountLegalEntity accountLegalEntity)
        {
            using (var transaction = _dataContext.Database.BeginTransaction())
            {

                var entity = await _dataContext.AccountLegalEntities.SingleOrDefaultAsync(c =>
                    c.AccountLegalEntityId.Equals(accountLegalEntity.AccountLegalEntityId));

                if (entity != null)
                {
                    _dataContext.AccountLegalEntities.Remove(entity);
                    _dataContext.SaveChanges();
                }
                transaction.Commit();
            }
        }
    }
}
