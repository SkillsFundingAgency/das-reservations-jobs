using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services
{
    public class WhenNotifyingEmployerOfNewReservation
    {
        [Test, MoqAutoData]
        public async Task Then_Sends_To_Notifications_Api(
            NotificationMessage createdMessage,
            [Frozen] Mock<HttpClient> mockNotificationClient,
            NotificationsService service)
        {
            //await service.SendNewReservationMessage(createdMessage);

            //var email = new Email
            //{
            //    RecipientsAddress = createdMessage.RecipientsAddress,
            //    Subject = "x",
            //    SystemId = "x",
            //    ReplyToAddress = "noreply@sfa.gov.uk",
            //    TemplateId = createdMessage.TemplateId,
            //    Tokens = createdMessage.Tokens
            //};


            //mockNotificationClient.Verify(api => api.PostAsync("api/email",
            //    It.IsAny<StringContent>()));

            //c => c.ReadAsStringAsync().Result.Equals(JsonConvert.SerializeObject(email)
        }
    }
}