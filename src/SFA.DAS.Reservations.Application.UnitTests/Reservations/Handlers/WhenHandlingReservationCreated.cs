using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Handlers
{
    public class WhenHandlingReservationCreated
    {
        [Test, MoqAutoData]
        public async Task And_No_ProviderId_Then_No_Further_Processing(
            ReservationCreatedEvent createdEvent,
            [Frozen] Mock<IProviderService> mockProviderService,
            ReservationCreatedHandler handler)
        {
            createdEvent.ProviderId = null;

            await handler.Handle(createdEvent);

            mockProviderService.Verify(service => service.GetDetails(It.IsAny<uint>()),
                Times.Never);
        }

        [Test, MoqAutoData]
        public async Task And_Has_ProviderId_Then_Gets_ProviderName(
            ReservationCreatedEvent createdEvent,
            [Frozen] Mock<IProviderService> mockProviderService,
            ReservationCreatedHandler handler)
        {
            await handler.Handle(createdEvent);

            mockProviderService.Verify(service => service.GetDetails(createdEvent.ProviderId.Value), 
                Times.Once);
        }
    }
}