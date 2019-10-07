using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class ReservationCreatedHandler : IReservationCreatedHandler
    {
        private readonly IAccountsService _accountsService;
        private readonly INotificationsService _notificationsService;
        private readonly ReservationsJobs _config;
        private readonly ILogger<ReservationCreatedHandler> _logger;
        private readonly INotificationTokenBuilder _notificationTokenBuilder;
        private readonly string[] _permittedRoles = {"Owner", "Transactor"};
        

        public ReservationCreatedHandler(
            IAccountsService accountsService,
            INotificationsService notificationsService,
            IOptions<ReservationsJobs> options,
            ILogger<ReservationCreatedHandler> logger, 
            INotificationTokenBuilder notificationTokenBuilder)
        {
            _accountsService = accountsService;
            _notificationsService = notificationsService;
            _config = options.Value;
            _logger = logger;
            _notificationTokenBuilder = notificationTokenBuilder;
        }

        public async Task Handle(ReservationCreatedEvent createdEvent)
        {
            _logger.LogDebug($"Handling Reservation Created, Id [{createdEvent.Id}].");

            if (!createdEvent.ProviderId.HasValue)
            {
                _logger.LogDebug("Reservation is not created by provider, no further processing.");
                return;
            }

            var users = await _accountsService.GetAccountUsers(createdEvent.AccountId);

            _logger.LogDebug($"Account [{createdEvent.AccountId}] has [{users.Count()}] users in total.");

            var filteredUsers = users.Where(user => 
                user.CanReceiveNotifications && 
                _permittedRoles.Contains(user.Role));

            _logger.LogDebug($"Account [{createdEvent.AccountId}] has [{users.Count()}] users with correct role and subscription.");

            var sendCount = 0;
            var tokens = await _notificationTokenBuilder.BuildTokens(createdEvent);
            foreach (var user in filteredUsers)
            {
                var message = new ReservationCreatedMessage
                {
                    RecipientsAddress = user.Email,
                    TemplateId = _config.ReservationCreatedEmailTemplateId,
                    Tokens = tokens
                };

                _notificationsService.SendNewReservationMessage(message);
                sendCount++;
            }

            _logger.LogDebug($"Finished handling Reservation Created, [{sendCount}] email(s) sent.");
        }
    }
}