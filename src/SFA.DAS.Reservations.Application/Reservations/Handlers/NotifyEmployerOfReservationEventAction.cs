using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Application.Reservations.Services;
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
        logger.LogInformation("Notify employer of action [{EventTypeName}], Reservation Id [{NotificationEventId}].", notificationEvent.GetType().Name, notificationEvent.Id);

        if (EventIsNotForProcessing(notificationEvent))
        {
            return;
        }

        var teamMembers = (await accountsService.GetAccountUsers(notificationEvent.AccountId)).ToList();

        logger.LogInformation("Reservation [{NotificationEventId}], Account [{AccountId}] has [{TeamMembersCount}] users in total.", notificationEvent.Id, notificationEvent.AccountId, teamMembers.Count);

        var filteredUsers = teamMembers.Where(user => user.CanReceiveNotifications && _permittedRoles.Contains(user.Role)).ToList();

        logger.LogInformation("Reservation [{NotificationEventId}], Account [{AccountId}] has [{FilteredUsersCount}] users with correct role and subscription.", notificationEvent.Id, notificationEvent.AccountId, filteredUsers.Count);
        
        var sendCount = await SendNotifications(notificationEvent, filteredUsers);

        logger.LogInformation("Finished notifying employer of action [{EventTypeName}], Reservation Id [{NotificationEventId}], [{SendCount}] email(s) sent.", notificationEvent.GetType().Name, notificationEvent.Id, sendCount);
    }

    private bool EventIsNotForProcessing<T>(T notificationEvent) where T : INotificationEvent
    {
        if (EventIsNotFromProvider(notificationEvent))
        {
            logger.LogInformation("Reservation [{NotificationEventId}] is not created by provider, no further processing.", notificationEvent.Id);
            return true;
        }

        if (EventIsFromEmployerDelete(notificationEvent))
        {
            logger.LogInformation("Reservation [{NotificationEventId}] is created by provider but deleted by employer, no further processing.", notificationEvent.Id);
            return true;
        }

        if (EventIsFromLevyAccount(notificationEvent))
        {
            logger.LogInformation("Reservation [{NotificationEventId}] is from levy account, no further processing.", notificationEvent.Id);
            return true;
        }

        return false;
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

                await notificationsService.SendEmail(message);
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