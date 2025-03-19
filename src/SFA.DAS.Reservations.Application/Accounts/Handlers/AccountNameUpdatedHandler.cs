using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.Accounts.Handlers
{
    public class AccountNameUpdatedHandler(IAccountsService accountsService) : IAccountNameUpdatedHandler
    {
        public async Task Handle(ChangedAccountNameEvent accountNameChangeEvent)
        {
            await accountsService.UpdateAccountName(accountNameChangeEvent.AccountId,
                accountNameChangeEvent.CurrentName);
        }
    }
}