using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.OuterApi.Requests;

namespace SFA.DAS.Reservations.Application.UnitTests.OuterApi.Requests;

public class WhenBuildingGetCoursesRequest
{
    [Test]
    public void Then_The_Url_Is_Correct()
    {
        var actual = new GetCoursesRequest();

        actual.GetUrl.Should().Be("trainingcourses");
    }
}