using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class AccountLegalEntityRepository : IAccountLegalEntityRepository
    {
        private const int UniqueConstraintViolation = 2601;
        private const int UniqueKeyViolation = 2627;
        private readonly IReservationsDataContext _dataContext;
        private readonly ILogger<AccountLegalEntityRepository> _log;

        public AccountLegalEntityRepository(IReservationsDataContext dataContext, ILogger<AccountLegalEntityRepository> log)
        {
            _dataContext = dataContext;
            _log = log;
        }

        public async Task Add(AccountLegalEntity accountLegalEntity)
        {
            using (var transaction = _dataContext.Database.BeginTransaction())
            {
                var existingEntity = await _dataContext.AccountLegalEntities.SingleOrDefaultAsync(c =>
                    c.AccountLegalEntityId.Equals(accountLegalEntity.AccountLegalEntityId));

                if (existingEntity != null)
                {
                    return;
                }

                try
                {
                    await _dataContext.AccountLegalEntities.AddAsync(accountLegalEntity);
                    _dataContext.SaveChanges();
                    transaction.Commit();
                }
                catch (DbUpdateException e)
                {
                    if (e.GetBaseException() is SqlException sqlException
                        && (sqlException.Number == UniqueConstraintViolation || sqlException.Number == UniqueKeyViolation))
                    {
                        _log.LogWarning($"AccountLegalEntityRepository: Rolling back Id:{accountLegalEntity.AccountLegalEntityId} - item already exists.");
                        transaction.Rollback();
                    }
                }
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