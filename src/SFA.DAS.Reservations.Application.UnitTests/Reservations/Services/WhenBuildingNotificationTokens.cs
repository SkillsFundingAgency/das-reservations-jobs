using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services
{
    public class WhenBuildingNotificationTokens
    {
        [Test, MoqAutoData]
        public async Task Then_Gets_ProviderName(
            ReservationCreatedEvent createdEvent,
            [Frozen] Mock<IProviderService> mockProviderService,
            NotificationTokenBuilder builder)
        {
            await builder.BuildTokens(createdEvent);

            mockProviderService.Verify(service => service.GetDetails(createdEvent.ProviderId.Value), 
                Times.Once);
        }
    }
}