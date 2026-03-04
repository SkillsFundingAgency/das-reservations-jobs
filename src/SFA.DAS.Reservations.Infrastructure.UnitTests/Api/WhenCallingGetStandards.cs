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

public class WhenCallingGetCourses
{
    [Test, MoqAutoData]
    public async Task Then_The_Endpoint_Is_Called_With_Api_Key_And_Courses_Returned(CourseApiResponse response, Mock<IOuterApiClient> outerApiClient)
    {
        outerApiClient.Setup(x => x.Get<CourseApiResponse>(new GetCoursesRequest())).ReturnsAsync(response);

        var sut = new ReferenceDataImportService(outerApiClient.Object);

        var result = await sut.GetCourses();

        result.Should().BeEquivalentTo(response);
    }

    [Test, AutoData]
    public void Then_If_It_Is_Not_Successful_An_Exception_Is_Thrown(Mock<IOuterApiClient> outerApiClient)
    {
        outerApiClient.Setup(x => x.Get<CourseApiResponse>(new GetCoursesRequest())).Throws<HttpRequestException>();

        var sut = new ReferenceDataImportService(outerApiClient.Object);

        var result = async () => await sut.GetCourses();

        result.Should().ThrowAsync<HttpRequestException>();
    }
}