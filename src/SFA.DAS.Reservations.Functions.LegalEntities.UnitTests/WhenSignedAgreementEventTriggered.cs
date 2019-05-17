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
    public class WhenSignedAgreementEventTriggered
    {
        [Test]
        public async Task Then_Queue_Message_Will_Be_Created()
        {
            //Arrange
            var queueService = new Mock<IAzureQueueService>();
            var message = new KeyValuePair<string, string>("SFA.DAS.EmployerAccounts.Messages.Events.SignedAgreementEvent", "");

            //Act
            await HandleManageYourApprenticeshipsEvents.Run(message, queueService.Object, Mock.Of<ILogger>());

            //Assert
            queueService.Verify(s => s.SendMessage(It.IsAny<SignedAgreementEvent>(), QueueNames.SignedAgreement), Times.Once);
        }

        [Test]
        public async Task Then_Queue_Message_Will_Not_Be_Created_If_Not_Of_Expected_Type()
        {
            //Arrange
            var queueService = new Mock<IAzureQueueService>();
            var message = new KeyValuePair<string, string>(typeof(WhenSignedAgreementEventTriggered).ToString(), "");

            //Act
            await HandleManageYourApprenticeshipsEvents.Run(message, queueService.Object, Mock.Of<ILogger>());

            //Assert
            queueService.Verify(s => s.SendMessage(It.IsAny<SignedAgreementEvent>(), QueueNames.SignedAgreement), Times.Never);
        }
    }
}
