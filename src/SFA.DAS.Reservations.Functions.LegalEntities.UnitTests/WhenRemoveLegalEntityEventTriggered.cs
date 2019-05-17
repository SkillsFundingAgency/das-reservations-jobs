using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.LegalEntities.UnitTests
{
    public class WhenRemoveLegalEntityEventTriggered
    {
        [Test]
        public async Task Then_Queue_Message_Will_Be_Created()
        {
            //Arrange
            var queueService = new Mock<IAzureQueueService>();
            var message = new KeyValuePair<string, string>("SFA.DAS.EmployerAccounts.Messages.Events.RemovedLegalEntityEvent", "");

            //Act
            await HandleManageYourApprenticeshipsEvents.Run(message, queueService.Object, Mock.Of<ILogger>());

            //Assert
            queueService.Verify(s => s.SendMessage(It.IsAny<AccountLegalEntityRemovedEvent>(), QueueNames.RemovedLegalEntity), Times.Once);
        }

        [Test]
        public async Task Then_Queue_Message_Will_Not_Be_Created_If_Not_Of_Expected_Type()
        {
            //Arrange
            var queueService = new Mock<IAzureQueueService>();
            var message = new KeyValuePair<string, string>(typeof(WhenRemoveLegalEntityEventTriggered).ToString(), "");

            //Act
            await HandleManageYourApprenticeshipsEvents.Run(message, queueService.Object, Mock.Of<ILogger>());

            //Assert
            queueService.Verify(s => s.SendMessage(It.IsAny<AccountLegalEntityRemovedEvent>(), QueueNames.RemovedLegalEntity), Times.Never);
        }
    }
}
