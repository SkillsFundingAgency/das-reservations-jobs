using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.Accounts.Handlers
{
    public class AddAccountHandler : IAddAccountHandler
    {
        private readonly IAccountsService _accountsService;

        public AddAccountHandler (IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }
        public async Task Handle(CreatedAccountEvent createdAccount)
        {
            await _accountsService.CreateAccount(createdAccount.AccountId, createdAccount.Name);
        }
    }
}