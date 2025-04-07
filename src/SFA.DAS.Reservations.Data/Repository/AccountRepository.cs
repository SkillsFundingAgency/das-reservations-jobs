using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class AccountRepository(IReservationsDataContext dataContext, ILogger<AccountRepository> logger)
        : IAccountRepository
    {
        private const int UniqueConstraintViolation = 2601;
        private const int UniqueKeyViolation = 2627;

        public async Task Add(Account account)
        {

            var existingEntity = await dataContext.Accounts.FindAsync(account.Id);

            if (existingEntity != null)
            {
                return;
            }

            try
            {
                await dataContext.Accounts.AddAsync(account);
                dataContext.SaveChanges();

            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is SqlException sqlException
                    && (sqlException.Number == UniqueConstraintViolation || sqlException.Number == UniqueKeyViolation))
                {
                    logger.LogWarning($"AccountRepository: Rolling back Id:{account.Id} - item already exists.");

                }
            }

        }

        public async Task UpdateName(Account account)
        {

            var entity = await dataContext.Accounts.FindAsync(account.Id);

            if (entity != null)
            {
                entity.Name = account.Name;
                dataContext.SaveChanges();
            }
            else
            {
                throw new DbUpdateException($"Update Account Name - Record not found AccountId:{account.Id}", (Exception)null);
            }

        }

        public async Task UpdateLevyStatus(Account account)
        {

            var entity = await dataContext.Accounts.FindAsync(account.Id);

            if (entity != null)
            {
                entity.IsLevy = account.IsLevy;
                dataContext.SaveChanges();
            }
            else
            {
                throw new DbUpdateException($"Update Account Levy Status - Record not found AccountId:{account.Id}", (Exception)null);
            }

        }
    }
}