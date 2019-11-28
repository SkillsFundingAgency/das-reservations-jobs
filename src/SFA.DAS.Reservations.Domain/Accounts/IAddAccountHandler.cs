using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.Reservations.Domain.Accounts
{
    public interface IAddAccountHandler
    {
        Task Handle(CreatedAccountEvent createdAccount);
    }
}