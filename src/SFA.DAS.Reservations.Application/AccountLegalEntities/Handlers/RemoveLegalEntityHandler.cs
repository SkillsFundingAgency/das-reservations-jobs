using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers
{
    public class RemoveLegalEntityHandler(IAccountLegalEntitiesService service) : IRemoveLegalEntityHandler
    {
        public async Task Handle(RemovedLegalEntityEvent accountLegalEntityRemovedEvent)
        {
            await service.RemoveAccountLegalEntity(accountLegalEntityRemovedEvent);
        }
    }
}
