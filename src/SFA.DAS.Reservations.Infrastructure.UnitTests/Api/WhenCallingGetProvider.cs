using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Application.OuterApi.Requests;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.ImportTypes;
using SFA.DAS.Reservations.Infrastructure.Api;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Infrastructure.UnitTests.Api;

public class WhenCallingGetProvider
{
    [Test, MoqAutoData]
    public async Task Then_The_Endpoint_Is_Called_With_Api_Key_And_Providers_Returned(
        uint ukPrn,
        ProviderApiResponse providerApiResponse,
        Mock<IOuterApiClient> outerApiClient)
    {
        outerApiClient.Setup(x => x.Get<ProviderApiResponse>(new GetProviderRequest(ukPrn))).ReturnsAsync(providerApiResponse);

        var sut = new FindApprenticeshipTrainingService(outerApiClient.Object);

        var result = await sut.GetProvider(ukPrn);

        result.Should().BeEquivalentTo(providerApiResponse);
    }

    [Test, AutoData]
    public void And_Response_Not_200_Then_Exception_Is_Thrown(
        uint ukPrn,
        Mock<IOuterApiClient> outerApiClient)
    {
        outerApiClient.Setup(x => x.Get<ProviderApiResponse>(new GetProviderRequest(ukPrn))).Throws<HttpRequestException>();

        var sut = new FindApprenticeshipTrainingService(outerApiClient.Object);

        var result = async () => await sut.GetProvider(ukPrn);
        
        result.Should().ThrowAsync<HttpRequestException>();
    }
}