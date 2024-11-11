using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Functions.LegalEntities.Functions;

namespace SFA.DAS.Reservations.Functions.LegalEntities.UnitTests
{
    [TestFixture]
    public class WhenLevyAddedToAccountEventTriggered
    {
        [Test]
        public async Task ThenMessageIsHandled()
        {
            //Arrange
            var message = new LevyAddedToAccount
            {
                AccountId = 1234345,
                Amount = 23423.3m,
                Created = DateTime.Now.AddDays(-1)
            };
            var handler = new Mock<ILevyAddedToAccountHandler>();
            var logger = new Mock<ILogger<LevyAddedToAccount>>();
            var sut = new HandleLevyAddedToAccountEvent(handler.Object, logger.Object);

            //Act
            await sut.Handle(message, Mock.Of<IMessageHandlerContext>());

            //Assert
            handler.Verify(x => x.Handle(message), Times.Once);
        }
    }
}
