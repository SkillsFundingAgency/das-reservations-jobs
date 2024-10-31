using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Functions.LegalEntities.UnitTests;

public class WhenRemoveLegalEntityEventTriggered
{
    [Test]
    public async Task Then_Queue_Message_Will_Be_Handled()
    {
        //Arrange
        var handle = new Mock<IRemoveLegalEntityHandler>();
        var message = new RemovedLegalEntityEvent{AccountId = 5432};

        //Act
        await HandleRemovedLegalEntityEvent.Run(message, handle.Object, Mock.Of<ILogger<RemovedLegalEntityEvent>>());

        //Assert
        handle.Verify(s => s.Handle(It.Is<RemovedLegalEntityEvent>(c=>c.AccountId.Equals(message.AccountId))), Times.Once);
    }
}