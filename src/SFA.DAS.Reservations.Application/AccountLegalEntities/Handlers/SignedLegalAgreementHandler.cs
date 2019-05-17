using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers
{
    public class SignedLegalAgreementHandler : ISignedLegalAgreementHandler
    {
        private readonly IAccountLegalEntitiesService _service;

        public SignedLegalAgreementHandler(IAccountLegalEntitiesService service)
        {
            _service = service;
        }

        public async Task Handle(SignedAgreementEvent signedAgreementEvent)
        {
            await _service.SignAgreementForAccountLegalEntity(signedAgreementEvent);
        }
    }
}
