using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Accounts;

public interface IAccountsService
{
    Task<IEnumerable<TeamMember>> GetAccountUsers(long accountId);
    Task CreateAccount(long accountId, string name);
    Task UpdateAccountName(long accountId, string name);
    Task UpdateLevyStatus(long accountId, bool isLevy);
}