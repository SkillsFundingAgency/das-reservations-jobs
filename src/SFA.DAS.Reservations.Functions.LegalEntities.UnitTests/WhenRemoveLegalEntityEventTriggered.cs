using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Functions.LegalEntities.Functions;

namespace SFA.DAS.Reservations.Functions.LegalEntities.UnitTests
{
    public class WhenRemoveLegalEntityEventTriggered
    {
        [Test]
        public async Task Then_Queue_Message_Will_Be_Handled()
        {
            //Arrange
            var handle = new Mock<IRemoveLegalEntityHandler>();
            var message = new RemovedLegalEntityEvent { AccountId = 5432 };

            var sut = new HandleRemovedLegalEntityEvent(handle.Object, Mock.Of<ILogger<RemovedLegalEntityEvent>>());

            //Act
            await sut.Handle(message, Mock.Of<IMessageHandlerContext>());

            //Assert
            handle.Verify(s => s.Handle(It.Is<RemovedLegalEntityEvent>(c => c.AccountId.Equals(message.AccountId))), Times.Once);
        }

    }
}
