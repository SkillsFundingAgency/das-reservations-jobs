using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Functions.LegalEntities.UnitTests
{
    public class WhenLegalEntityAddedEventTriggered
    {
        [Test]
        public async Task Then_Message_Will_Be_Handled()
        {
            //Arrange
            var handler = new Mock<IAddAccountLegalEntityHandler>();
            var message = new AddedLegalEntityEvent{AccountId = 123};

            //Act
            await HandleAddedLegalEntityEvent.Run(message, handler.Object, Mock.Of<ILogger<AddedLegalEntityEvent>>());

            //Assert
            handler.Verify(s => s.Handle(It.Is<AddedLegalEntityEvent>(c=>c.AccountId.Equals(message.AccountId))), Times.Once);
        }
        
    }
}
