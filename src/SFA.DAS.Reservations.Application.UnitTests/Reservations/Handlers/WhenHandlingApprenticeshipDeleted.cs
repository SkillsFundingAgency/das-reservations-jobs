using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Handlers
{
    public class WhenHandlingApprenticeshipDeleted
    {
        [Test, MoqAutoData]
        public async Task Then_Calls_Service_With_ReservationId(
            Guid deletedEventGuid,
            [Frozen] Mock<IReservationService> mockReservationService,
            ApprenticeshipDeletedHandler handler)
        {
            await handler.Handle(deletedEventGuid);

            mockReservationService
                .Verify(service => service.UpdateReservationStatus(
                        deletedEventGuid, 
                        ReservationStatus.Pending,
                        null,
                        null,
                        null), 
                    Times.Once);
        }
    }
}