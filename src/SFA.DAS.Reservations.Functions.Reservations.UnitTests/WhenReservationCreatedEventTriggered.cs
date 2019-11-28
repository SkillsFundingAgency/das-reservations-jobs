using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Functions.Reservations.UnitTests
{
    public class WhenReservationCreatedEventTriggered
    {
        [Test, AutoData]
        public async Task Then_Notification_Action_Executed(
            int courseLevel,
            ReservationCreatedEvent createdEvent)
        {
            //Arrange
            createdEvent.CourseLevel = courseLevel.ToString();

            var updateIndexAction =
                new Mock<IAddNonLevyReservationToReservationsIndexAction>();

            var notifyAction = new Mock<INotifyEmployerOfReservationEventAction>();

            var handler = new ReservationCreatedHandler(updateIndexAction.Object,
                notifyAction.Object);

            //Act
            await HandleReservationCreatedEvent.Run(
                createdEvent,
                Mock.Of<ILogger<ReservationCreatedEvent>>(),
                handler);

            //Assert
            notifyAction.Verify(s => s.Execute(It.Is<ReservationCreatedNotificationEvent>(ev =>
                ev.Id == createdEvent.Id)), Times.Once);
        }

        [Test, AutoData]
        public async Task Then_Update_Index_Action_Executed(
            int courseLevel,
            ReservationCreatedEvent createdEvent)//TODO Mock the handler and do setup on it
        {
            //Arrange
            createdEvent.CourseLevel = courseLevel.ToString();

            var addNonLevyReservationToReservationsIndexAction =
                new Mock<IAddNonLevyReservationToReservationsIndexAction>();

            var notifyEmployerOfReservationEventAction = new Mock<INotifyEmployerOfReservationEventAction>();

            var handler = new ReservationCreatedHandler(addNonLevyReservationToReservationsIndexAction.Object,
                notifyEmployerOfReservationEventAction.Object);

            //Act
            await HandleReservationCreatedEvent.Run(
                createdEvent,
                Mock.Of<ILogger<ReservationCreatedEvent>>(),
                handler);

            //Assert
            addNonLevyReservationToReservationsIndexAction.Verify(s => s.Execute(It.Is<Reservation>(index =>
                index.Id == createdEvent.Id)), Times.Once);
        }
    }
}