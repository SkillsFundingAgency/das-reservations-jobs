using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class ReservationCreatedHandler : IReservationCreatedHandler
    {
        private readonly IProviderService _providerService;
        private readonly IAccountsService _accountsService;

        public ReservationCreatedHandler(
            IProviderService providerService,
            IAccountsService accountsService)
        {
            _providerService = providerService;
            _accountsService = accountsService;
        }

        public async Task Handle(ReservationCreatedEvent createdEvent)
        {
            if (!createdEvent.ProviderId.HasValue)
                return;

            await _providerService.GetDetails(createdEvent.ProviderId.Value);

            await _accountsService.GetAccountUsers(createdEvent.AccountId);
        }
    }
}