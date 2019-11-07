using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers
{
    public class LevyAddedToAccountHandler : ILevyAddedToAccountHandler
    {
        private readonly IAccountLegalEntitiesService _accountLegalEntitiesService;

        public LevyAddedToAccountHandler(IAccountLegalEntitiesService accountLegalEntitiesService)
        {
            _accountLegalEntitiesService = accountLegalEntitiesService;
        }

        public async Task Handle(LevyAddedToAccount levyAddedToAccountEvent)
        {
            await _accountLegalEntitiesService.UpdateAccountLegalEntitiesToLevy(levyAddedToAccountEvent);
        }
    }
}
