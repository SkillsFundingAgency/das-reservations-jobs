using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ReservationCreatedHandler> _logger;
        private readonly string[] _permittedRoles = {"Owner", "Transactor"};

        public ReservationCreatedHandler(
            IProviderService providerService,
            IAccountsService accountsService,
            INotificationsService notificationsService,
            ILogger<ReservationCreatedHandler> logger)
        {
            _providerService = providerService;
            _accountsService = accountsService;
            _notificationsService = notificationsService;
            _logger = logger;
        }

        public async Task Handle(ReservationCreatedEvent createdEvent)
        {
            _logger.LogDebug($"Handling Reservation Created, Id [{createdEvent.Id}].");

            if (!createdEvent.ProviderId.HasValue)
            {
                _logger.LogDebug("Reservation is not created by provider, no further processing.");
                return;
            }
                

            await _providerService.GetDetails(createdEvent.ProviderId.Value);

            var users = await _accountsService.GetAccountUsers(createdEvent.AccountId);

            _logger.LogDebug($"Account [{createdEvent.AccountId}] has [{users.Count()}] users in total.");

            var filteredUsers = users.Where(user => 
                user.CanReceiveNotifications && 
                _permittedRoles.Contains(user.Role));

            _logger.LogDebug($"Account [{createdEvent.AccountId}] has [{users.Count()}] users with correct role and subscription.");

            var sendCount = 0;
            foreach (var user in filteredUsers)
            {
                var message = new ReservationCreatedMessage
                {
                    RecipientsAddress = user.Email
                };

                _notificationsService.SendNewReservationMessage(message);
                sendCount++;
            }

            _logger.LogDebug($"Finished handling Reservation Created, [{sendCount}] email(s) sent.");
        }
    }
}