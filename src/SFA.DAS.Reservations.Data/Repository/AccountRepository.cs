using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private const int UniqueConstraintViolation = 2601;
        private const int UniqueKeyViolation = 2627;
        private readonly IReservationsDataContext _dataContext;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(IReservationsDataContext dataContext, ILogger<AccountRepository> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public async Task Add(Account account)
        {
            using (var transaction = new TransactionScope())
            {
                var existingEntity = await _dataContext.Accounts.FindAsync(account.Id);

                if (existingEntity != null)
                {
                    return;
                }

                try
                {
                    await _dataContext.Accounts.AddAsync(account);
                    _dataContext.SaveChanges();
                    transaction.Complete();
                }
                catch (DbUpdateException e)
                {
                    if (e.GetBaseException() is SqlException sqlException
                        && (sqlException.Number == UniqueConstraintViolation || sqlException.Number == UniqueKeyViolation))
                    {
                        _logger.LogWarning($"AccountRepository: Rolling back Id:{account.Id} - item already exists.");
                    }
                }
            }
        }

        public async Task UpdateName(Account account)
        {
            using (var transaction = new TransactionScope())
            {
                var entity = await _dataContext.Accounts.FindAsync(account.Id);

                if (entity != null)
                {
                    entity.Name = account.Name;
                    _dataContext.SaveChanges();
                }
                else
                {
                    throw new DbUpdateException($"Update Account Name - Record not found AccountId:{account.Id}", (Exception)null);
                }

                transaction.Complete();
            }
        }

        public async Task UpdateLevyStatus(Account account)
        {
            using (var transaction = new TransactionScope())
            {
                var entity = await _dataContext.Accounts.FindAsync(account.Id);

                if (entity != null)
                {
                    entity.IsLevy = account.IsLevy;
                    _dataContext.SaveChanges();
                }
                else
                {
                    throw new DbUpdateException($"Update Account Levy Status - Record not found AccountId:{account.Id}", (Exception)null);
                }

                transaction.Complete();
            }
        }
    }
}