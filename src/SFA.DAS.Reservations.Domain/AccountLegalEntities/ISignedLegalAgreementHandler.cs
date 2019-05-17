using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public interface ISignedLegalAgreementHandler
    {
        Task Handle(SignedAgreementEvent accountLegalEntityAddedEvent);
    }
}