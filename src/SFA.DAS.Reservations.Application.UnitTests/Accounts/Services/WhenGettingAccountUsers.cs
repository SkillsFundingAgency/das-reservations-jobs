using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Accounts.Services
{
    public class WhenGettingAccountUsers
    {
        [Test, MoqAutoData]
        public async Task Then_Calls_Api(
            long accountId,
            [Frozen] Mock<IAccountApiClient> mockAccountApiClient,
            AccountsService service)
        {
            await service.GetAccountUsers(accountId);

            mockAccountApiClient.Verify(client => client.GetAccountUsers(accountId), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Users_From_Api(
            long accountId,
            List<TeamMemberViewModel> apiTeamMembers,
            [Frozen] Mock<IAccountApiClient> mockAccountApiClient,
            AccountsService service)
        {
            mockAccountApiClient
                .Setup(client => client.GetAccountUsers(accountId))
                .ReturnsAsync(apiTeamMembers);

            var response = await service.GetAccountUsers(accountId);

            response.Should().BeEquivalentTo(apiTeamMembers);
        }
    }
}