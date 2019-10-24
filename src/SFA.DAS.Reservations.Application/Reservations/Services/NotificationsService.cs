using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly HttpClient _client;
        private const string Placeholder = "x";
        private const string DummyReplyAddress = "noreply@sfa.gov.uk";

        public NotificationsService(HttpClient client)
        {
            _client = client;
        }

        public async Task SendNewReservationMessage(NotificationMessage message)
        {
            var email = new Email(
                Placeholder,
                message.TemplateId, 
                Placeholder, 
                message.RecipientsAddress, 
                DummyReplyAddress, 
                message.Tokens);

            await _client.PostAsync("api/email", new StringContent(JsonConvert.SerializeObject(email),System.Text.Encoding.UTF8,"application/json"));
        }
    }
}