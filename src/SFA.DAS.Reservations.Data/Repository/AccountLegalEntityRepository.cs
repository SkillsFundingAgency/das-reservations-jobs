using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
    }
}
