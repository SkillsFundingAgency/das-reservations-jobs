using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Functions.Reservations.UnitTests
{
    public class WhenReservationDeletedEventTriggered
    {
        [Test, AutoData]
        public async Task Then_Notification_Action_Executed(ReservationDeletedEvent deletedEvent)
        {
            //Arrange
            var handler = new Mock<INotifyEmployerWhenReservationDeletedAction>();

            //Act
            await HandleReservationDeletedEvent.Run(
                deletedEvent, 
                Mock.Of<ILogger<ReservationDeletedEvent>>(),
                handler.Object);

            //Assert
            handler.Verify(s => s.Execute(deletedEvent), Times.Once);
        }
    }
}