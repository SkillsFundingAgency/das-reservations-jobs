using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers
{
    public class RemoveLegalEntityHandler
    {
        private readonly IAccountLegalEntitiesService _service;

        public RemoveLegalEntityHandler(IAccountLegalEntitiesService service)
        {
            _service = service;
        }

        public async Task Handler(AccountLegalEntityRemovedEvent accountLegalEntityRemovedEvent)
        {
            await _service.RemoveAccountLegalEntity(accountLegalEntityRemovedEvent);
        }
    }
}
