using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public interface IAccountLegalEntitiesService
    {
        Task AddAccountLegalEntity(AccountLegalEntityAddedEvent accountLegalEntity);
        Task SignAgreementForAccountLegalEntity(SignedAgreementEvent signedAgreementEvent);
        Task RemoveAccountLegalEntity(AccountLegalEntityRemovedEvent accountLegalEntityRemovedEvent);
    }
}
