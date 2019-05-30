using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class AccountLegalEntityRepository : IAccountLegalEntityRepository
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
                    c.AccountId.Equals(accountLegalEntity.AccountId) &&
                    c.LegalEntityId.Equals(accountLegalEntity.LegalEntityId));

                if (entity != null)
                {
                    entity.AgreementSigned = true;
                    _dataContext.SaveChanges();
                }
                else
                {
                    throw new DbUpdateException($"Record not found AccountId:{accountLegalEntity.AccountId} LegalEntityId:{accountLegalEntity.LegalEntityId}", (Exception) null);
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