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
        public async Task Then_Notification_Action_Executed_And_Index_Updated(ReservationDeletedEvent deletedEvent)
        {
            ReservationDeletedNotificationEvent actionArgument;
            //Arrange
            var handler = new Mock<INotifyEmployerOfReservationEventAction>();
            handler
                .Setup(action => action.Execute(It.IsAny<ReservationDeletedNotificationEvent>()))
                .Callback((ReservationDeletedNotificationEvent actualArgument) =>
                {
                    actionArgument = actualArgument;
                });
            var reservationService = new Mock<IReservationService>();

            //Act
            await HandleReservationDeletedEvent.Run(
                deletedEvent, 
                Mock.Of<ILogger<ReservationDeletedEvent>>(),
                reservationService.Object,
                handler.Object);

            //Assert
            handler.Verify(s => s.Execute(It.Is<ReservationDeletedNotificationEvent>(ev => 
                ev.Id == deletedEvent.Id)), Times.Once);
            reservationService.Verify(x=>x.UpdateReservationStatus(deletedEvent.Id, ReservationStatus.Deleted));
        }
    }
}