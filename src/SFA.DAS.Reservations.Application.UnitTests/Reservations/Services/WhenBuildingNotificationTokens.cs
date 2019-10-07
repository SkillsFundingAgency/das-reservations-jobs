using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
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
        public async Task Then_Adds_ProviderName_To_Tokens(
            ReservationCreatedEvent createdEvent,
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
        public async Task Then_Adds_StartDate_Description_To_Tokens(
            ReservationCreatedEvent createdEvent,
            NotificationTokenBuilder builder)
        {
            var tokens = await builder.BuildTokens(createdEvent);

            tokens[TokenKeyNames.StartDateDescription].Should()
                .Be($"{createdEvent.StartDate:MMM yyyy} to {createdEvent.EndDate:MMM yyyy}");
        }

        [Test, MoqAutoData]
        public async Task Then_Adds_Course_Description_To_Tokens(
            ReservationCreatedEvent createdEvent,
            NotificationTokenBuilder builder)
        {
            var tokens = await builder.BuildTokens(createdEvent);

            tokens[TokenKeyNames.CourseDescription].Should()
                .Be($"{createdEvent.CourseName} level {createdEvent.CourseLevel}");
        }

        [Test, MoqAutoData, Ignore("todo")]
        public async Task Then_Adds_Manage_Url_To_Tokens(
            ReservationCreatedEvent createdEvent,
            string encodedAccountId,
            //[Frozen] Mock<IEncodingService> mockEncodingService,
            NotificationTokenBuilder builder)
        {
            var tokens = await builder.BuildTokens(createdEvent);

            tokens[TokenKeyNames.ManageUrl].Should()
                .Be($"{encodedAccountId}");
        }
    }
}