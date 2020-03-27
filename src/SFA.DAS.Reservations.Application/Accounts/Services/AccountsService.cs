using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Application.Accounts.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly IAccountRepository _accountRepository;

        public AccountsService(IAccountApiClient accountApiClient, IAccountRepository accountRepository)
        {
            _accountApiClient = accountApiClient;
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<UserDetails>> GetAccountUsers(long accountId)
        {
            var teamMembers = await _accountApiClient.GetAccountUsers(accountId);
            var users = teamMembers.Select<TeamMemberViewModel, UserDetails>(model => model);
            return users;
        }

        public async Task CreateAccount(long accountId, string name)
        {
            await _accountRepository.Add(new Account
            {
                Id = accountId,
                Name = name
            });
        }

        public async Task UpdateAccountName(long accountId, string name)
        {
            await _accountRepository.UpdateName(new Account
            {
                Id = accountId,
                Name = name
            });
        }

        public async Task UpdateLevyStatus(long accountId, bool isLevy)
        {
            await _accountRepository.UpdateLevyStatus(new Account
            {
                Id = accountId,
                IsLevy = isLevy
            });
        }
    }
}