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
        [Frozen] Mock<IOuterApiClient> outerApiClient,
        GetAccountUsersResponse response,
        AccountsService service)
    {
        outerApiClient
            .Setup(x => x.Get<GetAccountUsersResponse>(It.Is<GetAccountUsersRequest>(y => y.GetUrl.Equals($"accounts/{accountId}/users"))))
            .ReturnsAsync(response);

        await service.GetAccountUsers(accountId);

        outerApiClient.Verify(client => client.Get<GetAccountUsersResponse>(It.Is<GetAccountUsersRequest>(y => y.GetUrl.Equals($"accounts/{accountId}/users"))), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_Users_From_Api(
        long accountId,
        [Frozen] Mock<IOuterApiClient> outerApiClient,
        GetAccountUsersResponse response,
        AccountsService service)
    {
        outerApiClient
            .Setup(client => client.Get<GetAccountUsersResponse>(It.Is<GetAccountUsersRequest>(y => y.GetUrl.Equals($"accounts/{accountId}/users"))))
            .ReturnsAsync(response);

        var result = (await service.GetAccountUsers(accountId)).ToList();

        result
            .Should()
            .BeEquivalentTo(response.AccountUsers, options => options
                .Excluding(c => c.Status));

        foreach (var accountUser in result)
        {
            result
                .Should()
                .Contain(userDetails => userDetails.Status.Equals(accountUser.Status));
        }
    }
}