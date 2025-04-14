using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers
{
    public class LevyAddedToAccountHandler(
        IAccountsService accountLegalEntitiesService,
        ILogger<LevyAddedToAccountHandler> logger)
        : ILevyAddedToAccountHandler
    {
        public async Task Handle(LevyAddedToAccountEvent levyAddedToAccountEvent)
        {
            try
            {
                await accountLegalEntitiesService.UpdateLevyStatus(levyAddedToAccountEvent.AccountId,true);
            }
            catch (DbUpdateException e)
            {
                logger.LogWarning("Could not update agreement status to signed", e);
                throw;
            }
        }
    }
}
