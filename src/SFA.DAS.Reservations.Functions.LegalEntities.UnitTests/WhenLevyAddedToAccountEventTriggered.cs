using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

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
            //Act
            await HandleLevyAddedToAccountEvent.Run(message, handler.Object,logger.Object);

            //Assert
            handler.Verify(x => x.Handle(message),Times.Once);
        }
    }
}
