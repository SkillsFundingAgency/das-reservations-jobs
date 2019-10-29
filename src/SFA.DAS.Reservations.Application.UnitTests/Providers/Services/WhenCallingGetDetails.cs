using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Reservations.Application.Providers.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Providers.Services
{
    public class WhenCallingGetDetails
    {
        [Test, MoqAutoData]
        public async Task Then_Gets_Provider_Details_From_Api(
            uint providerId,
            Provider providerFromApi,
            [Frozen] Mock<IProviderApiClient> mockApiClient,
            ProviderService service)
        {
            mockApiClient
                .Setup(client => client.GetAsync(providerId))
                .ReturnsAsync(providerFromApi);

            var providerDetails = await service.GetDetails(providerId);

            providerDetails.Should().BeEquivalentTo(providerFromApi, options => 
                options.ExcludingMissingMembers());
        }
    }
}