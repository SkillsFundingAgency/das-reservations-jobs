using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class NotifyEmployerOfReservationEventAction : INotifyEmployerOfReservationEventAction
    {
        private readonly IAccountsService _accountsService;
        private readonly INotificationsService _notificationsService;
        private readonly ILogger<NotifyEmployerOfReservationEventAction> _logger;
        private readonly INotificationTokenBuilder _notificationTokenBuilder;
        private readonly string[] _permittedRoles = {"Owner", "Transactor"};
        

        public NotifyEmployerOfReservationEventAction(
            IAccountsService accountsService,
            INotificationsService notificationsService,
            ILogger<NotifyEmployerOfReservationEventAction> logger, 
            INotificationTokenBuilder notificationTokenBuilder)
        {
            _accountsService = accountsService;
            _notificationsService = notificationsService;
            _logger = logger;
            _notificationTokenBuilder = notificationTokenBuilder;
        }

        public async Task Execute<T>(T notificationEvent) where T : INotificationEvent
        {
            _logger.LogInformation($"Notify employer of action [{notificationEvent.GetType().Name}], Reservation Id [{notificationEvent.Id}].");

            if (EventIsNotFromProvider(notificationEvent))
            {
                _logger.LogInformation($"Reservation [{notificationEvent.Id}] is not created by provider, no further processing.");
                return;
            }

            if (EventIsFromEmployerDelete(notificationEvent))
            {
                _logger.LogInformation($"Reservation [{notificationEvent.Id}] is created by provider but deleted by employer, no further processing.");
                return;
            }

            if (EventIsFromLevyAccount(notificationEvent))
            {
                _logger.LogInformation($"Reservation [{notificationEvent.Id}] is from levy account, no further processing.");
                return;
            }

            var users = await _accountsService.GetAccountUsers(notificationEvent.AccountId);

            _logger.LogInformation($"Reservation [{notificationEvent.Id}], Account [{notificationEvent.AccountId}] has [{users.Count()}] users in total.");

            var filteredUsers = users.Where(user => 
                user.CanReceiveNotifications && 
                _permittedRoles.Contains(user.Role)).ToList();

            _logger.LogInformation($"Reservation [{notificationEvent.Id}], Account [{notificationEvent.AccountId}] has [{filteredUsers.Count}] users with correct role and subscription.");

            var sendCount = 0;
            var tokens = await _notificationTokenBuilder.BuildTokens(notificationEvent);
            foreach (var user in filteredUsers)
            {
                var message = new NotificationMessage
                {
                    RecipientsAddress = user.Email,
                    TemplateId = GetTemplateName(notificationEvent),
                    Tokens = tokens
                };

                await _notificationsService.SendEmail(message);
                sendCount++;
            }

            _logger.LogInformation($"Finished notifying employer of action [{notificationEvent.GetType().Name}], Reservation Id [{notificationEvent.Id}], [{sendCount}] email(s) sent.");
        }

        private static bool EventIsNotFromProvider(INotificationEvent notificationEvent)
        {
            return !notificationEvent.ProviderId.HasValue;
        }

        private static bool EventIsFromLevyAccount(INotificationEvent notificationEvent)
        {
            return notificationEvent.CourseId == null && notificationEvent.StartDate == DateTime.MinValue;
        }

        private static bool EventIsFromEmployerDelete(INotificationEvent notificationEvent)
        {
            return notificationEvent.ProviderId.HasValue && notificationEvent.EmployerDeleted;
        }

        private string GetTemplateName(INotificationEvent notificationEvent)
        {
            switch (notificationEvent.GetType().Name)
            {
                case nameof(ReservationCreatedNotificationEvent): return TemplateIds.ReservationCreated;
                case nameof(ReservationDeletedNotificationEvent): return TemplateIds.ReservationDeleted;
                default: throw new NotImplementedException("No template found for this event.");
            }
        }
    }
}