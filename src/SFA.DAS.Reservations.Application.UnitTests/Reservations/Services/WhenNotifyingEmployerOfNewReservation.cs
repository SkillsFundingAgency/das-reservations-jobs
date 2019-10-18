using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services
{
    public class WhenNotifyingEmployerOfNewReservation
    {
        [Test, MoqAutoData]
        public async Task Then_Sends_To_Notifications_Api(
            NotificationMessage createdMessage,
            [Frozen] Mock<INotificationsApi> mockNotificationClient,
            NotificationsService service)
        {
            service.SendNewReservationMessage(createdMessage);
            
            mockNotificationClient.Verify(api => api.SendEmail(It.Is<Email>(email => 
                email.RecipientsAddress == createdMessage.RecipientsAddress &&
                email.Subject == "x" && 
                email.SystemId == "x" &&
                email.ReplyToAddress == "noreply@sfa.gov.uk" &&
                email.TemplateId == createdMessage.TemplateId &&
                email.Tokens ==  createdMessage.Tokens)));
        }
    }
}