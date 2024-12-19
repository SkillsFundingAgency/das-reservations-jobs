using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Functions.Reservations.Functions;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Functions.Reservations.UnitTests
{
    public class WhenReservationDeletedEventTriggered
    {
        [Test, AutoData]
        public async Task Then_Notification_Action_Executed_And_Index_Updated(ReservationDeletedEvent deletedEvent)
        {
            //Arrange
            var reservationService = new Mock<IReservationService>();
            var notifyAction = new Mock<INotifyEmployerOfReservationEventAction>();
            var handler = new ReservationDeletedHandler(reservationService.Object, notifyAction.Object);
            var sut = new HandleReservationDeletedEvent(handler, Mock.Of<ILogger<ReservationDeletedEvent>>());

            //Act
            await sut.Handle(deletedEvent, Mock.Of<IMessageHandlerContext>());

            //Assert
            notifyAction.Verify(s => s.Execute(It.Is<ReservationDeletedNotificationEvent>(ev =>
                ev.Id == deletedEvent.Id), It.IsAny<IMessageHandlerContext>()), Times.Once);
            reservationService.Verify(x => x.UpdateReservationStatus(
                deletedEvent.Id,
                ReservationStatus.Deleted,
                null,
                null,
                null));
        }
    }
}