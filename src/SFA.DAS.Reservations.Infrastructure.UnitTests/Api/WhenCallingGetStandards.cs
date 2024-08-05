using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Application.OuterApi.Requests;
using SFA.DAS.Reservations.Domain.ImportTypes;
using SFA.DAS.Reservations.Infrastructure.Api;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Infrastructure.UnitTests.Api;

public class WhenCallingGetStandards
{
    [Test, MoqAutoData]
    public async Task Then_The_Endpoint_Is_Called_With_Api_Key_And_Standards_Returned(StandardApiResponse response, Mock<IOuterApiClient> outerApiClient)
    {
        outerApiClient.Setup(x => x.Get<StandardApiResponse>(new GetStandardsRequest())).ReturnsAsync(response);

        var sut = new FindApprenticeshipTrainingService(outerApiClient.Object);

        var result = await sut.GetStandards();

        result.Should().BeEquivalentTo(response);
    }

    [Test, AutoData]
    public void Then_If_It_Is_Not_Successful_An_Exception_Is_Thrown(Mock<IOuterApiClient> outerApiClient)
    {
        outerApiClient.Setup(x => x.Get<StandardApiResponse>(new GetStandardsRequest())).Throws<HttpRequestException>();

        var sut = new FindApprenticeshipTrainingService(outerApiClient.Object);

        var result = async () => await sut.GetStandards();

        result.Should().ThrowAsync<HttpRequestException>();
    }
}