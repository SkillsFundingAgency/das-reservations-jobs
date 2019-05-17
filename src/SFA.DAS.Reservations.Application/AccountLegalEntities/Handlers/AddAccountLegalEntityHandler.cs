using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers
{
    public class AddAccountLegalEntityHandler : IAddAccountLegalEntityHandler
    {
        private readonly IAccountLegalEntitiesService _service;

        public AddAccountLegalEntityHandler(IAccountLegalEntitiesService service)
        {
            _service = service;
        }

        public async Task Handle(AccountLegalEntityAddedEvent accountLegalEntityAddedEvent)
        {
            await _service.AddAccountLegalEntity(accountLegalEntityAddedEvent);
        }
    }
}
