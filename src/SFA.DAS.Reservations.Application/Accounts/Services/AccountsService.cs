using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.Accounts.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IAccountApiClient _accountApiClient;

        public AccountsService(IAccountApiClient accountApiClient)
        {
            _accountApiClient = accountApiClient;
        }

        public async Task<IEnumerable<UserDetails>> GetAccountUsers(long accountId)
        {
            var teamMembers = await _accountApiClient.GetAccountUsers(accountId);
            var users = teamMembers.Select<TeamMemberViewModel, UserDetails>(model => model);
            return users;
        }
    }
}