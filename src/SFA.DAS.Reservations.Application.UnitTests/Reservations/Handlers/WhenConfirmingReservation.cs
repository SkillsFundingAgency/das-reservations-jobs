using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
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
            var action = () => handler.Handle(createdEvent);
            action.Should()
                .ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(DraftApprenticeshipCreatedEvent.ReservationId));

            //Assert
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
