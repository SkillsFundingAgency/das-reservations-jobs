using System;
using System.Collections.Generic;
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
    public class NotifyEmployerWhenReservationDeletedAction : INotifyEmployerWhenReservationDeletedAction
    {
        private readonly IAccountsService _accountsService;
        private readonly INotificationsService _notificationsService;
        private readonly ReservationsJobs _config;
        private readonly ILogger<ReservationCreatedHandler> _logger;
        private readonly INotificationTokenBuilder _notificationTokenBuilder;
        private readonly string[] _permittedRoles = {"Owner", "Transactor"};
        

        public NotifyEmployerWhenReservationDeletedAction(
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

        public async Task Execute(ReservationDeletedEvent deletedEvent)
        {
            _logger.LogInformation($"Notify employer that reservation deleted, Reservation Id [{deletedEvent.Id}].");

            if (EventIsNotFromProvider(deletedEvent))
            {
                _logger.LogInformation($"Reservation [{deletedEvent.Id}] is not created by provider, no further processing.");
                return;
            }

            if (EventIsFromLevyAccount(deletedEvent))
            {
                _logger.LogInformation($"Reservation [{deletedEvent.Id}] is from levy account, no further processing.");
                return;
            }

            var users = await _accountsService.GetAccountUsers(deletedEvent.AccountId);

            _logger.LogInformation($"Account [{deletedEvent.AccountId}] has [{users.Count()}] users in total.");

            var filteredUsers = users.Where(user => 
                user.CanReceiveNotifications && 
                _permittedRoles.Contains(user.Role)).ToList();

            _logger.LogInformation($"Account [{deletedEvent.AccountId}] has [{filteredUsers.Count}] users with correct role and subscription.");

            var sendCount = 0;
            var tokens = await _notificationTokenBuilder.BuildReservationDeletedTokens(deletedEvent);
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

            _logger.LogInformation($"Finished notifying employer that reservation deleted, Reservation Id [{deletedEvent.Id}], [{sendCount}] email(s) sent.");
        }

        private static bool EventIsNotFromProvider(ReservationDeletedEvent deletedEvent)
        {
            return !deletedEvent.ProviderId.HasValue;
        }

        private static bool EventIsFromLevyAccount(ReservationDeletedEvent deletedEvent)
        {
            return deletedEvent.CourseId == null && deletedEvent.StartDate == DateTime.MinValue;
        }
    }
}