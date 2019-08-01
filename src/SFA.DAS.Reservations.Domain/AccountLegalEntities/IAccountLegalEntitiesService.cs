using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Messages.Events;

namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public interface IAccountLegalEntitiesService
    {
        Task AddAccountLegalEntity(AddedLegalEntityEvent accountLegalEntity);
        Task SignAgreementForAccountLegalEntity(EmployerAccounts.Messages.Events.SignedAgreementEvent signedAgreementEvent);
        Task RemoveAccountLegalEntity(RemovedLegalEntityEvent accountLegalEntityRemovedEvent);
        Task UpdateAccountToLevy(LevyAddedToAccount levyAddedToAccountEvent);
    }
}
