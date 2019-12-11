using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.Accounts.Handlers
{
    public class AccountNameUpdatedHandler : IAccountNameUpdatedHandler
    {
        private readonly IAccountsService _accountsService;

        public AccountNameUpdatedHandler (IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }
        public async Task Handle(ChangedAccountNameEvent accountNameChangeEvent)
        {
            await _accountsService.UpdateAccountName(accountNameChangeEvent.AccountId,
                accountNameChangeEvent.CurrentName);
        }
    }
}