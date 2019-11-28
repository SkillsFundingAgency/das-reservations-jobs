using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Application.Accounts.Handlers;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Accounts.Handlers
{
    public class WhenHandlingAddingAccounts
    {
        [Test, MoqAutoData]
        public async Task Then_The_Service_Is_Called_To_Add_The_Entity(
            CreatedAccountEvent createdAccountEvent,
            [Frozen]Mock<IAccountsService> service,
            AddAccountHandler handler
            )
        {
            //Act
            await handler.Handle(createdAccountEvent);
            
            //Assert
            service.Verify(x=>x.CreateAccount(createdAccountEvent.AccountId, createdAccountEvent.Name));
        }
    }
}