using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services
{
    public class WhenBuildingTokens
    {
        [Test, MoqAutoData]
        public async Task Then_Adds_ProviderName_To_Tokens(
            ReservationCreatedNotificationEvent createdEvent,
            ProviderDetails provider,
            [Frozen] Mock<IProviderService> mockProviderService,
            NotificationTokenBuilder builder)
        {
            mockProviderService
                .Setup(service => service.GetDetails(createdEvent.ProviderId.Value))
                .ReturnsAsync(provider);

            var tokens = await builder.BuildTokens(createdEvent);

            tokens[TokenKeyNames.ProviderName].Should().Be(provider.ProviderName);
        }

        [Test, MoqAutoData]
        public async Task Then_Adds_StartDateDescription_To_Tokens(
            ReservationCreatedNotificationEvent createdEvent,
            NotificationTokenBuilder builder)
        {
            var tokens = await builder.BuildTokens(createdEvent);

            tokens[TokenKeyNames.StartDateDescription].Should()
                .Be($"{createdEvent.StartDate:MMM yyyy} to {createdEvent.EndDate:MMM yyyy}");
        }

        [Test, MoqAutoData]
        public async Task Then_Adds_CourseDescription_To_Tokens(
            ReservationDeletedNotificationEvent deletedEvent,
            NotificationTokenBuilder builder)
        {
            var tokens = await builder.BuildTokens(deletedEvent);

            tokens[TokenKeyNames.CourseDescription].Should()
                .Be($"{deletedEvent.CourseName} level {deletedEvent.CourseLevel}");
        }

        [Test, MoqAutoData]
        public async Task Then_Adds_HashedAccountId_To_Tokens(
            ReservationDeletedNotificationEvent deletedEvent,
            string encodedAccountId,
            [Frozen] Mock<IEncodingService> mockEncodingService,
            NotificationTokenBuilder builder)
        {
            mockEncodingService
                .Setup(service => service.Encode(deletedEvent.AccountId, EncodingType.AccountId))
                .Returns(encodedAccountId);

            var tokens = await builder.BuildTokens(deletedEvent);

            tokens[TokenKeyNames.HashedAccountId].Should().Be(encodedAccountId);
        }
    }
}