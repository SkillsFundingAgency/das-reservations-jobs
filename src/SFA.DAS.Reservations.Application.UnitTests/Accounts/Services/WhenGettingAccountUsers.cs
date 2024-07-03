using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Accounts.Services;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Application.OuterApi.Requests;
using SFA.DAS.Reservations.Application.OuterApi.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Accounts.Services;

public class WhenGettingAccountUsers
{
    [Test, MoqAutoData]
    public async Task Then_Calls_Api(
        long accountId,
        Mock<IOuterApiClient> outerApiClient,
        ICollection<AccountUser> apiTeamMembers,
        GetAccountUsersResponse response,
        AccountsService service)
    {
        response.AccountUsers = apiTeamMembers;
        
        //outerApiClient.Setup(x => x.Get<GetAccountUsersResponse>(It.Is<GetAccountUsersRequest>(y => y.Equals(accountId)))).ReturnsAsync(response);
        outerApiClient.Setup(x => x.Get<GetAccountUsersResponse>(It.IsAny<GetAccountUsersRequest>())).ReturnsAsync(response);
        
        await service.GetAccountUsers(accountId);

        outerApiClient.Verify(client => client.Get<GetAccountUsersResponse>(new GetAccountUsersRequest(accountId)), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_Users_From_Api(
        long accountId,
        List<AccountUser> apiTeamMembers,
        Mock<IOuterApiClient> outerApiClient,
        AccountsService service)
    {
        outerApiClient
            .Setup(client => client.Get<GetAccountUsersResponse>(new GetAccountUsersRequest(accountId)))
            .ReturnsAsync(new GetAccountUsersResponse
            {
                AccountUsers = apiTeamMembers
            });

        var response = (await service.GetAccountUsers(accountId)).ToList();

        response.Should().BeEquivalentTo(apiTeamMembers, options => options
            .Excluding(c => c.Status)
        );

        foreach (var exp in apiTeamMembers)
        {
            response.Should().Contain(act =>
                act.Status.Equals((byte)exp.Status)
            );
        }
    }
}