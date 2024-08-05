using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.OuterApi.Requests;

namespace SFA.DAS.Reservations.Application.UnitTests.OuterApi.Requests;

public class WhenBuildingGetAccountUsersRequest
{
    [Test, AutoData]
    public void Then_The_Url_Is_Correctly_Formatted_With_AccountId(long accountId)
    {
        var actual = new GetAccountUsersRequest(accountId);
        actual.GetUrl.Should().Be($"accounts/{accountId}/users");
    }
}