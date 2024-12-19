using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Functions.LegalEntities.Functions;

namespace SFA.DAS.Reservations.Functions.LegalEntities.UnitTests
{
    public class WhenSignedAgreementEventTriggered
    {
        [Test]
        public async Task Then_Queue_Message_Will_Be_Created()
        {
            //Arrange
            var handler = new Mock<ISignedLegalAgreementHandler>();
            var message = new SignedAgreementEvent { AccountId = 432 };
            var sut = new HandleSignedAgreementEvent(handler.Object, Mock.Of<ILogger<SignedAgreementEvent>>());

            //Act
            await sut.Handle(message, Mock.Of<IMessageHandlerContext>());

            //Assert
            handler.Verify(s => s.Handle(It.Is<SignedAgreementEvent>(c => c.AccountId.Equals(message.AccountId))), Times.Once);
        }

    }
}
