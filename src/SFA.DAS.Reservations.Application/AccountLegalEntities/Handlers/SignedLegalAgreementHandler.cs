using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers
{
    public class SignedLegalAgreementHandler(
        IAccountLegalEntitiesService service,
        ILogger<SignedLegalAgreementHandler> logger)
        : ISignedLegalAgreementHandler
    {
        public async Task Handle(SignedAgreementEvent signedAgreementEvent)
        {
            try
            {
                await service.SignAgreementForAccountLegalEntity(signedAgreementEvent);
            }
            catch (DbUpdateException e)
            {
                logger.LogWarning("Could not update levy status to is levy payer", e);
                throw;
            }
        }
    }
}
