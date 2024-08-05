using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.OuterApi.Requests;

namespace SFA.DAS.Reservations.Application.UnitTests.OuterApi.Requests;

public class WhenBuildingGetStandardsRequest
{
    [Test, AutoData]
    public void Then_The_Url_Is_Correct()
    {
        var actual = new GetStandardsRequest();
        
        actual.GetUrl.Should().Be("trainingcourses");
    }
}