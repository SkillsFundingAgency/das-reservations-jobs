using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public interface IAddAccountLegalEntityHandler
    {
        Task Handle(AddedLegalEntityEvent accountLegalEntityAddedEvent);
    }
}
