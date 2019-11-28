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
    public class WhenHandlingUpdatingAccounts
    {
        [Test, MoqAutoData]
        public async Task Then_The_Service_Is_Called_To_Update_The_Entity(
            ChangedAccountNameEvent changedAccountNameEvent,
            [Frozen]Mock<IAccountsService> service,
            AccountNameUpdatedHandler handler)
        {
            //Act
            await handler.Handle(changedAccountNameEvent);
            
            //Assert
            service.Verify(x=>x.UpdateAccountName(changedAccountNameEvent.AccountId, changedAccountNameEvent.CurrentName));
        }
    }
}