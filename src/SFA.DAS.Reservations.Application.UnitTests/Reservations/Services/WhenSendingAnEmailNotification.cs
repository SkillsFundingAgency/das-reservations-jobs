using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Notifications.Messages.Commands;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services;

public class WhenSendingAnEmailNotification
{
    [Test, MoqAutoData]
    public async Task Then_The_MessageSession_Send_Method_Is_Called(
        NotificationMessage message,
        [Frozen] Mock<IMessageSession> publisher,
        NotificationsService service)
    {
        await service.SendEmail(message);

        publisher.Verify(x => x.Send(It.Is<SendEmailCommand>(c =>
                c.TemplateId == message.TemplateId
                && c.RecipientsAddress == message.RecipientsAddress
                && c.Tokens == message.Tokens),
            It.IsAny<SendOptions>()
        ), Times.Once);
    }
}