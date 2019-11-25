using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers
{
    public class LevyAddedToAccountHandler : ILevyAddedToAccountHandler
    {
        private readonly IAccountLegalEntitiesService _accountLegalEntitiesService;
        private readonly ILogger<LevyAddedToAccountHandler> _logger;

        public LevyAddedToAccountHandler(IAccountLegalEntitiesService accountLegalEntitiesService,
            ILogger<LevyAddedToAccountHandler> logger)
        {
            _accountLegalEntitiesService = accountLegalEntitiesService;
            _logger = logger;
        }

        public async Task Handle(LevyAddedToAccount levyAddedToAccountEvent)
        {
            try
            {
                await _accountLegalEntitiesService.UpdateAccountLegalEntitiesToLevy(levyAddedToAccountEvent);
            }
            catch (DbUpdateException e)
            {
                _logger.LogWarning("Could not update agreement status to signed", e);
            }
        }
    }
}
