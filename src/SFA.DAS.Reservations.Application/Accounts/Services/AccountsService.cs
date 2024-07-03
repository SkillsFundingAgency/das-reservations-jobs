using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Application.Accounts.Services;

public class AccountsService(
    IAccountApiClient accountApiClient,
    IAccountRepository accountRepository,
    IOuterApiClient outerApiClient) : IAccountsService
{
    public async Task<IEnumerable<UserDetails>> GetAccountUsers(long accountId)
    {
        var teamMembers = await accountApiClient.GetAccountUsers(accountId);
        return teamMembers.Select<TeamMemberViewModel, UserDetails>(model => model);
    }

    public async Task CreateAccount(long accountId, string name)
    {
        await accountRepository.Add(new Account
        {
            Id = accountId,
            Name = name
        });
    }

    public async Task UpdateAccountName(long accountId, string name)
    {
        await accountRepository.UpdateName(new Account
        {
            Id = accountId,
            Name = name
        });
    }

    public async Task UpdateLevyStatus(long accountId, bool isLevy)
    {
        await accountRepository.UpdateLevyStatus(new Account
        {
            Id = accountId,
            IsLevy = isLevy
        });
    }
}