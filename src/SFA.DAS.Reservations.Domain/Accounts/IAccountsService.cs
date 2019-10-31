using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Accounts
{
    public interface IAccountsService
    {
        Task<IEnumerable<UserDetails>> GetAccountUsers(long accountId);
    }
}