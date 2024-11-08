using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Functions.Reservations.UnitTests
{
    public class WhenApprenticeshipDeletedEventTriggered
    {
        [Test]
        public async Task Then_Message_Handler_Called()
        {
            //Arrange
            var handler = new Mock<IApprenticeshipDeletedHandler>();
            var message = new DraftApprenticeshipDeletedEvent { ReservationId = Guid.NewGuid() };
            var sut = new HandleApprenticeshipDeletedEvent(handler.Object, Mock.Of<ILogger<DraftApprenticeshipDeletedEvent>>());

            //Act
            await sut.Handle(message, Mock.Of<IMessageHandlerContext>());

            //Assert
            handler.Verify(s => s.Handle(message.ReservationId.Value), Times.Once);
        }
    }
}