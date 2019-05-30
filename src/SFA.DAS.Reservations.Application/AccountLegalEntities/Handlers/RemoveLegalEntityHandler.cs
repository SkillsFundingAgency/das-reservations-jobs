using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers
{
    public class RemoveLegalEntityHandler : IRemoveLegalEntityHandler
    {
        private readonly IAccountLegalEntitiesService _service;

        public RemoveLegalEntityHandler(IAccountLegalEntitiesService service)
        {
            _service = service;
        }

        public async Task Handle(RemovedLegalEntityEvent accountLegalEntityRemovedEvent)
        {
            await _service.RemoveAccountLegalEntity(accountLegalEntityRemovedEvent);
        }
    }
}
