using System.Threading.Tasks;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsApi _notificationsApi;
        private const string Placeholder = "x";
        private const string DummyReplyAddress = "noreply@sfa.gov.uk";

        public NotificationsService(INotificationsApi notificationsApi)
        {
            _notificationsApi = notificationsApi;
        }

        public async Task SendNewReservationMessage(ReservationCreatedMessage createdMessage)
        {
            var email = new Email(
                Placeholder,
                createdMessage.TemplateId, 
                Placeholder, 
                createdMessage.RecipientsAddress, 
                DummyReplyAddress, 
                createdMessage.Tokens);
            await _notificationsApi.SendEmail(email);
        }
    }
}