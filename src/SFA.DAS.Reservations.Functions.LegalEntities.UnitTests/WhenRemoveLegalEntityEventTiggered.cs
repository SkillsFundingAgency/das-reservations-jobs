using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Events;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.LegalEntities.UnitTests
{
    public class WhenRemoveLegalEntityEventTiggered
    {
        [Test]
        public async Task ThenQueueMessageWillBeCreated()
        {
            //Arrange
            var queueService = new Mock<IAzureQueueService>();
            var message = new KeyValuePair<string, string>(typeof(AccountLegalEntityRemovedEvent).ToString(), "");

            //Act
            await CreateLegalEntity.Run(message, queueService.Object, Mock.Of<ILogger>());

            //Assert
            queueService.Verify(s => s.SendMessage(It.IsAny<AccountLegalEntityRemovedEvent>(), QueueNames.RemovedLegalEntity), Times.Once);
        }

        [Test]
        public async Task ThenQueueMessageWillNotBeCreatedIfNotOfExpectedType()
        {
            //Arrange
            var queueService = new Mock<IAzureQueueService>();
            var message = new KeyValuePair<string, string>(typeof(WhenRemoveLegalEntityEventTiggered).ToString(), "");

            //Act
            await CreateLegalEntity.Run(message, queueService.Object, Mock.Of<ILogger>());

            //Assert
            queueService.Verify(s => s.SendMessage(It.IsAny<AccountLegalEntityRemovedEvent>(), QueueNames.RemovedLegalEntity), Times.Never);
        }
    }
}
