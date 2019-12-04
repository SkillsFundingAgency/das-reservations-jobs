using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.Reservations.Domain.Accounts
{
    public interface IAccountNameUpdatedHandler
    {
        Task Handle(ChangedAccountNameEvent accountNameChangeEvent);
    }
}