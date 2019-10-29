using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class AccountLegalEntityRepository : IAccountLegalEntityRepository
    {
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

                var existingLevyStatus = await _dataContext
                    .AccountLegalEntities
                    .Where(c => c.AccountId.Equals(accountLegalEntity.AccountId))
                    .AnyAsync(c=>c.IsLevy);

                accountLegalEntity.IsLevy = existingLevyStatus;
                try
                {
                    await _dataContext.AccountLegalEntities.AddAsync(accountLegalEntity);
                    _dataContext.SaveChanges();
                    transaction.Commit();
                }
                catch (DbUpdateException e)
                {
                    if (e.GetBaseException() is SqlException sqlException
                        && (sqlException.Number == 2601 || sqlException.Number == 2627))
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
                    entity.AgreementType = accountLegalEntity.AgreementType;
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

        public async Task UpdateAccountLegalEntitiesToLevy(AccountLegalEntity accountLegalEntity)
        {
            using (var transaction = _dataContext.Database.BeginTransaction())
            {
                var accountLegalEntitiesList = await _dataContext.AccountLegalEntities
                    .Where(x => x.AccountId.Equals(accountLegalEntity.AccountId))
                    .ToListAsync();

                if (accountLegalEntitiesList != null &&
                    accountLegalEntitiesList.Any())
                {
                    accountLegalEntitiesList.ForEach(entity => entity.IsLevy = true);
                    _dataContext.AccountLegalEntities.UpdateRange(accountLegalEntitiesList);
                    _dataContext.SaveChanges();
                }
                else
                {
                    throw new DbUpdateException($"Record not found AccountId:{accountLegalEntity.AccountId}", (Exception)null);
                }

                transaction.Commit();
            }
        }
    }
}