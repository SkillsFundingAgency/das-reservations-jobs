using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class AccountLegalEntityRepository(
        IReservationsDataContext dataContext,
        ILogger<AccountLegalEntityRepository> log)
        : IAccountLegalEntityRepository
    {
        private const int UniqueConstraintViolation = 2601;
        private const int UniqueKeyViolation = 2627;

        public async Task Add(AccountLegalEntity accountLegalEntity)
        {
           
                var existingEntity = await dataContext.AccountLegalEntities.SingleOrDefaultAsync(c =>
                    c.AccountLegalEntityId.Equals(accountLegalEntity.AccountLegalEntityId));

                if (existingEntity != null)
                {
                    return;
                }

                try
                {
                    await dataContext.AccountLegalEntities.AddAsync(accountLegalEntity);
                    dataContext.SaveChanges();
                   
                }
                catch (DbUpdateException e)
                {
                    if (e.GetBaseException() is SqlException sqlException
                        && (sqlException.Number == UniqueConstraintViolation || sqlException.Number == UniqueKeyViolation))
                    {
                        log.LogWarning($"AccountLegalEntityRepository: Rolling back Id:{accountLegalEntity.AccountLegalEntityId} - item already exists.");
                        
                    }
                }
            
        }

        public async Task UpdateAgreementStatus(AccountLegalEntity accountLegalEntity)
        {
          
                var entity = await dataContext.AccountLegalEntities.SingleOrDefaultAsync(c =>
                    c.AccountId.Equals(accountLegalEntity.AccountId) &&
                    c.LegalEntityId.Equals(accountLegalEntity.LegalEntityId));

                if (entity != null)
                {
                    entity.AgreementSigned = true;
                    dataContext.SaveChanges();
                }
                else
                {
                    throw new DbUpdateException($"Record not found AccountId:{accountLegalEntity.AccountId} LegalEntityId:{accountLegalEntity.LegalEntityId}", (Exception)null);
                }

        }

        public async Task Remove(AccountLegalEntity accountLegalEntity)
        {
            var entity = await dataContext.AccountLegalEntities.SingleOrDefaultAsync(c =>
                    c.AccountLegalEntityId.Equals(accountLegalEntity.AccountLegalEntityId));

                if (entity != null)
                {
                    dataContext.AccountLegalEntities.Remove(entity);
                    dataContext.SaveChanges();
                }
        }
        
    }
}