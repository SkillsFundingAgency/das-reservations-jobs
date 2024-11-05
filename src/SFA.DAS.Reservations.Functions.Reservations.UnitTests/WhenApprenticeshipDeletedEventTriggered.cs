//using System;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using Moq;
//using NUnit.Framework;
//using SFA.DAS.CommitmentsV2.Messages.Events;
//using SFA.DAS.Reservations.Domain.Reservations;

//namespace SFA.DAS.Reservations.Functions.Reservations.UnitTests
//{
//    public class WhenApprenticeshipDeletedEventTriggered
//    {
//        [Test]
//        public async Task Then_Message_Handler_Called()
//        {
//            //Arrange
//            var handler = new Mock<IApprenticeshipDeletedHandler>();
//            var message = new DraftApprenticeshipDeletedEvent{ReservationId = Guid.NewGuid()};

//            //Act
//            await HandleApprenticeshipDeletedEvent.Run(
//                message, 
//                Mock.Of<ILogger<DraftApprenticeshipDeletedEvent>>(),
//                handler.Object);

//            //Assert
//            handler.Verify(s => s.Handle(message.ReservationId.Value), Times.Once);
//        }
//    }
//}