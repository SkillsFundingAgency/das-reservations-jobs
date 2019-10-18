using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Notifications;
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
            _logger.LogInformation($"Notify employer that reservation created, Reservation Id [{createdEvent.Id}].");

            if (EventIsNotFromProvider(createdEvent))
            {
                _logger.LogInformation($"Reservation [{createdEvent.Id}] is not created by provider, no further processing.");
                return;
            }

            if (EventIsFromLevyAccount(createdEvent))
            {
                _logger.LogInformation($"Reservation [{createdEvent.Id}] is from levy account, no further processing.");
                return;
            }

            var users = await _accountsService.GetAccountUsers(createdEvent.AccountId);

            _logger.LogInformation($"Reservation [{createdEvent.Id}], Account [{createdEvent.AccountId}] has [{users.Count()}] users in total.");

            var filteredUsers = users.Where(user => 
                user.CanReceiveNotifications && 
                _permittedRoles.Contains(user.Role)).ToList();

            _logger.LogInformation($"Reservation [{createdEvent.Id}], Account [{createdEvent.AccountId}] has [{filteredUsers.Count}] users with correct role and subscription.");

            var sendCount = 0;
            var tokens = await _notificationTokenBuilder.BuildReservationCreatedTokens(createdEvent);
            foreach (var user in filteredUsers)
            {
                var message = new NotificationMessage
                {
                    RecipientsAddress = user.Email,
                    TemplateId = TemplateIds.ReservationCreated,
                    Tokens = tokens
                };

                await _notificationsService.SendNewReservationMessage(message);
                sendCount++;
            }

            _logger.LogInformation($"Finished notifying employer that reservation created, Reservation Id [{createdEvent.Id}], [{sendCount}] email(s) sent.");
        }

        private static bool EventIsNotFromProvider(ReservationCreatedEvent createdEvent)
        {
            return !createdEvent.ProviderId.HasValue;
        }

        private static bool EventIsFromLevyAccount(ReservationCreatedEvent createdEvent)
        {
            return createdEvent.CourseId == null && createdEvent.StartDate == DateTime.MinValue;
        }
    }
}