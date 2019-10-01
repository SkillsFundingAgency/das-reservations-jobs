using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Reservations.Domain.Accounts;
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

    public class AccountsService
    {
        private readonly IAccountApiClient _accountApiClient;

        public AccountsService(IAccountApiClient accountApiClient)
        {
            _accountApiClient = accountApiClient;
        }

        public async Task<IEnumerable<UserDetails>> GetAccountUsers(long accountId)
        {
            var teamMembers = await _accountApiClient.GetAccountUsers(accountId);
            var users = teamMembers.Select<TeamMemberViewModel, UserDetails>(model => model);
            return users;
        }
    }
}