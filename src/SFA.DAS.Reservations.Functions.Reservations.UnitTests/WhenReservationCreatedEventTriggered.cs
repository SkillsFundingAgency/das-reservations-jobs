using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Functions.Reservations.UnitTests
{
    public class WhenReservationCreatedEventTriggered
    {
        [Test, AutoData]
        public async Task Then_Message_Handler_Called(ReservationCreatedEvent createdEvent)
        {
            //Arrange
            var handler = new Mock<IReservationCreatedHandler>();

            //Act
            await HandleReservationCreatedEvent.Run(
                createdEvent, 
                Mock.Of<ILogger<ReservationCreatedEvent>>(),
                handler.Object);

            //Assert
            handler.Verify(s => s.Handle(createdEvent), Times.Once);
        }
    }
}