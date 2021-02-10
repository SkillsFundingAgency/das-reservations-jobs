using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Providers.Services;
using SFA.DAS.Reservations.Domain.ImportTypes;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Providers.Services
{
    public class WhenCallingGetDetails
    {
        [Test, MoqAutoData]
        public async Task Then_Gets_Provider_Details_From_Api(
            uint ukPrn,
            ProviderApiResponse providerFromApi,
            [Frozen] Mock<IFindApprenticeshipTrainingService> mockApiClient,
            ProviderService service)
        {
            mockApiClient
                .Setup(client => client.GetProvider(ukPrn))
                .ReturnsAsync(providerFromApi);

            var providerDetails = await service.GetDetails(ukPrn);

            providerDetails.Should().BeEquivalentTo((ProviderDetails)providerFromApi);
        }
    }
}