using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.Accounts.Handlers
{
    public class AddAccountHandler(IAccountsService accountsService) : IAddAccountHandler
    {
        public async Task Handle(CreatedAccountEvent createdAccount)
        {
            await accountsService.CreateAccount(createdAccount.AccountId, createdAccount.Name);
        }
    }
}