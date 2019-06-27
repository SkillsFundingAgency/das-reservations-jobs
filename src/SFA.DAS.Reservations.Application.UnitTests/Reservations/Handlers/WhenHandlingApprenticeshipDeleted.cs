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
    public class WhenHandlingApprenticeshipDeleted
    {
        [Test, MoqAutoData]
        public void And_No_ReservationId_Then_Throws_ArgumentException(
            DraftApprenticeshipDeletedEvent deletedEvent,
            ApprenticeshipDeletedHandler handler)
        {
            deletedEvent.ReservationId = null;

            var act = new Func<Task>(async () => await handler.Handle(deletedEvent));

            act.Should().Throw<ArgumentException>()
                .WithMessage($"ReservationId is missing from DraftApprenticeshipDeletedEvent, DraftApprenticeshipId:[{deletedEvent.DraftApprenticeshipId}].");
        }

        [Test, MoqAutoData]
        public async Task Then_Calls_Service_With_ReservationId(
            DraftApprenticeshipDeletedEvent deletedEvent,
            [Frozen] Mock<IReservationService> mockReservationService,
            ApprenticeshipDeletedHandler handler)
        {
            await handler.Handle(deletedEvent);

            mockReservationService
                .Verify(service => service.UpdateReservationStatus(
                        deletedEvent.ReservationId.Value, 
                        ReservationStatus.Pending), 
                    Times.Once);
        }
    }
}