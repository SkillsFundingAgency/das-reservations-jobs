using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Functions.LegalEntities.UnitTests
{
    public class WhenCreatedAccountEventTriggered
    {
        [Test]
        public async Task Then_The_Message_Will_Be_Handled()
        {
            //Arrange
            var handler = new Mock<IAddAccountHandler>();
            var message = new CreatedAccountEvent {AccountId = 1, Name = "Test"};
            
            //Act
            await HandleAccountAddedEvent.Run(message, handler.Object, Mock.Of<ILogger<CreatedAccountEvent>>());

            //Assert
            handler.Verify(
                x=>x.Handle(
                    It.Is<CreatedAccountEvent>(c=>c.Name.Equals(message.Name) 
                                                          && c.AccountId.Equals(message.AccountId))));
        }
    }
}