using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.OuterApi.Requests;

namespace SFA.DAS.Reservations.Application.UnitTests.OuterApi.Requests;

public class WhenBuildingGetProviderRequest
{
    [Test, AutoData]
    public void Then_The_Url_Is_Correctly_Formatted_With_UkPrn(uint ukPrn)
    {
        var actual = new GetProviderRequest(ukPrn);
        actual.GetUrl.Should().Be($"providers/{ukPrn}");
    }
}