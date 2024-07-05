using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers;

public class NotifyEmployerOfReservationEventAction(
    IAccountsService accountsService,
    INotificationsService notificationsService,
    ILogger<NotifyEmployerOfReservationEventAction> logger,
    INotificationTokenBuilder notificationTokenBuilder)
    : INotifyEmployerOfReservationEventAction
{
    private readonly string[] _permittedRoles = ["Owner", "Transactor"];

    public async Task Execute<T>(T notificationEvent) where T : INotificationEvent
    {
        logger.LogInformation($"Notify employer of action [{notificationEvent.GetType().Name}], Reservation Id [{notificationEvent.Id}].");

        if (EventIsNotFromProvider(notificationEvent))
        {
            logger.LogInformation($"Reservation [{notificationEvent.Id}] is not created by provider, no further processing.");
            return;
        }

        if (EventIsFromEmployerDelete(notificationEvent))
        {
            logger.LogInformation($"Reservation [{notificationEvent.Id}] is created by provider but deleted by employer, no further processing.");
            return;
        }

        if (EventIsFromLevyAccount(notificationEvent))
        {
            logger.LogInformation($"Reservation [{notificationEvent.Id}] is from levy account, no further processing.");
            return;
        }

        var users = await accountsService.GetAccountUsers(notificationEvent.AccountId);

        logger.LogInformation($"Reservation [{notificationEvent.Id}], Account [{notificationEvent.AccountId}] has [{users.Count()}] users in total.");

        var filteredUsers = users.Where(user => user.CanReceiveNotifications && _permittedRoles.Contains(user.Role)).ToList();

        logger.LogInformation($"Reservation [{notificationEvent.Id}], Account [{notificationEvent.AccountId}] has [{filteredUsers.Count}] users with correct role and subscription.");
        
        var sendCount = await SendNotifications(notificationEvent, filteredUsers);

        logger.LogInformation($"Finished notifying employer of action [{notificationEvent.GetType().Name}], Reservation Id [{notificationEvent.Id}], [{sendCount}] email(s) sent.");
    }

    private async Task<int> SendNotifications<T>(T notificationEvent, IEnumerable<TeamMember> filteredUsers) where T : INotificationEvent
    {
        var tokens = await notificationTokenBuilder.BuildTokens(notificationEvent);
        
        var sendCount = 0;
        
        foreach (var user in filteredUsers)
        {
            var message = new NotificationMessage
            {
                RecipientsAddress = user.Email,
                TemplateId = GetTemplateName(notificationEvent),
                Tokens = tokens
            };

            await notificationsService.SendNewReservationMessage(message);
            
            sendCount++;
        }

        return sendCount;
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

    private static string GetTemplateName(INotificationEvent notificationEvent)
    {
        return notificationEvent.GetType().Name switch
        {
            nameof(ReservationCreatedNotificationEvent) => TemplateIds.ReservationCreated,
            nameof(ReservationDeletedNotificationEvent) => TemplateIds.ReservationDeleted,
            _ => throw new NotImplementedException("No template found for this event.")
        };
    }
}