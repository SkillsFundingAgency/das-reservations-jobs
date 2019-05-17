using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public interface IRemoveLegalEntityHandler
    {
        Task Handle(AccountLegalEntityRemovedEvent accountLegalEntityAddedEvent);
    }
}