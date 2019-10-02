using System.Linq;
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
        private readonly INotificationsService _notificationsService;
        private readonly string[] _permittedRoles = {"Owner", "Transactor"};

        public ReservationCreatedHandler(
            IProviderService providerService,
            IAccountsService accountsService,
            INotificationsService notificationsService)
        {
            _providerService = providerService;
            _accountsService = accountsService;
            _notificationsService = notificationsService;
        }

        public async Task Handle(ReservationCreatedEvent createdEvent)
        {
            if (!createdEvent.ProviderId.HasValue)
                return;

            await _providerService.GetDetails(createdEvent.ProviderId.Value);

            var users = await _accountsService.GetAccountUsers(createdEvent.AccountId);

            var filteredUsers = users.Where(user => 
                user.CanReceiveNotifications && 
                _permittedRoles.Contains(user.Role));

            foreach (var user in filteredUsers)
            {
                var message = new ReservationCreatedMessage
                {
                    RecipientsAddress = user.Email
                };

                _notificationsService.SendNewReservationMessage(message);
            }
        }
    }
}