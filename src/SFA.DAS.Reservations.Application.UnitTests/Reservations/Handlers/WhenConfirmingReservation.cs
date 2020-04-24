using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Handlers
{
    public class WhenConfirmingReservation
    {
        [Test, MoqAutoData]
        public async Task ThenSetsReservationStatusToConfirmed(
            DraftApprenticeshipCreatedEvent createdEvent,
            [Frozen]Mock<IReservationService> mockService,
            ConfirmReservationHandler handler)
        {
            //Act
            await handler.Handle(createdEvent);

            //Assert
            mockService.Verify(s => s.UpdateReservationStatus(
                createdEvent.ReservationId.Value, 
                ReservationStatus.Confirmed,
                createdEvent.CreatedOn,
                createdEvent.CohortId,
                createdEvent.DraftApprenticeshipId), 
                Times.Once);
        }
        
        [Test, MoqAutoData]
        public void ThenWillThrowExceptionIfReservationIdIsInvalid(
            [Frozen]Mock<IReservationService> mockService,
            ConfirmReservationHandler handler)
        {
            //Arrange
            var createdEvent = new DraftApprenticeshipCreatedEvent(
                234, 
                234, 
                "345", 
                null, 
                DateTime.UtcNow);

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(createdEvent));

            //Assert
            Assert.AreEqual(nameof(DraftApprenticeshipCreatedEvent.ReservationId), exception.ParamName);
            mockService.Verify(s => s.UpdateReservationStatus(
                It.IsAny<Guid>(), 
                It.IsAny<ReservationStatus>(),
                It.IsAny<DateTime>(),
                It.IsAny<long>(),
                It.IsAny<long>()), 
                Times.Never);
        }
    }
}
