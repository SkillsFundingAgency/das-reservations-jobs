using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers
{
    public class SignedLegalAgreementHandler : ISignedLegalAgreementHandler
    {
        private readonly IAccountLegalEntitiesService _service;
        private readonly ILogger<SignedLegalAgreementHandler> _logger;

        public SignedLegalAgreementHandler(IAccountLegalEntitiesService service, ILogger<SignedLegalAgreementHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task Handle(SignedAgreementEvent signedAgreementEvent)
        {
            try
            {
                await _service.SignAgreementForAccountLegalEntity(signedAgreementEvent);
            }
            catch (DbUpdateException e)
            {
                _logger.LogWarning("Could not update levy status to is levy payer", e);
                throw;
            }
        }
    }
}
