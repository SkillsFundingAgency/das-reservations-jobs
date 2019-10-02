using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsApi _notificationsApi;

        public NotificationsService(INotificationsApi notificationsApi)
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