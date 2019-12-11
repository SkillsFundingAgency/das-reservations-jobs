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
    public class WhenStoringAccounts
    {
        [Test, MoqAutoData]
        public async Task Then_Calls_The_Repository_With_The_Account_Created(
            string accountName, 
            long accountId,
            [Frozen] Mock<IAccountRepository> repository,
            AccountsService service
        )
        {
            //Act
            await service.CreateAccount(accountId, accountName);

            //Assert
            repository.Verify(
                x => x.Add(It.Is<Account>(c =>
                    c.Id.Equals(accountId) && c.Name.Equals(accountName))),
                Times.Once);
        }
    }
}