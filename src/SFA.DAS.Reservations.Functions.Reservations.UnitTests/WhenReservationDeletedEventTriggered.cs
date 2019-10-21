using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Functions.Reservations.UnitTests
{
    public class WhenReservationDeletedEventTriggered
    {
        [Test, AutoData]
        public async Task Then_Notification_Action_Executed(ReservationDeletedEvent deletedEvent)
        {
            ReservationDeletedNotificationEvent actionArgument;
            //Arrange
            var handler = new Mock<INotifyEmployerWhenReservationDeletedAction>();
            handler
                .Setup(action => action.Execute(It.IsAny<ReservationDeletedNotificationEvent>()))
                .Callback((ReservationDeletedNotificationEvent actualArgument) =>
                {
                    actionArgument = actualArgument;
                });

            //Act
            await HandleReservationDeletedEvent.Run(
                deletedEvent, 
                Mock.Of<ILogger<ReservationDeletedEvent>>(),
                handler.Object);

            //Assert
            handler.Verify(s => s.Execute(It.Is<ReservationDeletedNotificationEvent>(ev => 
                ev.Id == deletedEvent.Id)), Times.Once);
        }
    }
}