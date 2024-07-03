using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Application.OuterApi.Requests;
using SFA.DAS.Reservations.Application.OuterApi.Responses;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Application.Accounts.Services;

public class AccountsService(
    IAccountRepository accountRepository,
    IOuterApiClient outerApiClient) : IAccountsService
{
    public async Task<IEnumerable<UserDetails>> GetAccountUsers(long accountId)
    {
        var response = await outerApiClient.Get<GetAccountUsersResponse>(new GetAccountUsersRequest(accountId));
        
        return response.AccountUsers.Select(source => new UserDetails
        {
            UserRef = source.UserRef,
            Name = source.Name,
            Email = source.Email,
            Role = source.Role,
            CanReceiveNotifications = source.CanReceiveNotifications,
            Status = (byte)source.Status
        });
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