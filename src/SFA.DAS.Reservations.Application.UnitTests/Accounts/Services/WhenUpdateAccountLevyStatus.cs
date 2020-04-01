using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Accounts.Services;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Accounts.Services
{
    public class WhenUpdateAccountLevyStatus
    {
        [Test, MoqAutoData]
        public async Task Then_Calls_The_Repository_With_Updated_Flag(
            bool isLevy,
            long accountId,
            [Frozen] Mock<IAccountRepository> repository,
            AccountsService service
        )
        {
            //Act
            await service.UpdateLevyStatus(accountId, isLevy);

            //Assert
            repository.Verify(
                x => x.UpdateLevyStatus(It.Is<Account>(c =>
                    c.Id.Equals(accountId) &&
                    c.IsLevy.Equals(isLevy))), Times.Once);
        }
    }
}