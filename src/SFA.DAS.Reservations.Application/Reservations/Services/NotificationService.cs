using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationsApi _notificationsApi;

        public NotificationService(INotificationsApi notificationsApi)
        {
            _notificationsApi = notificationsApi;
        }

        public void SendNewReservationMessage(ReservationCreatedMessage createdMessage)
        {
            var email = new Email(
                createdMessage.SystemId,
                createdMessage.TemplateId, 
                null, 
                createdMessage.RecipientsAddress, 
                null, 
                createdMessage.Tokens);
            _notificationsApi.SendEmail(email);
        }
    }
}