﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Infrastructure;

using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.LegalEntities.UnitTests
{
    public class WhenSignedAgreementEventTiggered
    {
        [Test]
        public async Task ThenQueueMessageWillBeCreated()
        {
            //Arrange
            var queueService = new Mock<IAzureQueueService>();
            var message = new KeyValuePair<string, string>(typeof(SignedAgreementEvent).ToString(), "");

            //Act
            await HandleManageYourApprenticeshipsEvents.Run(message, queueService.Object, Mock.Of<ILogger>());

            //Assert
            queueService.Verify(s => s.SendMessage(It.IsAny<SignedAgreementEvent>(), QueueNames.SignedAgreement), Times.Once);
        }

        [Test]
        public async Task ThenQueueMessageWillNotBeCreatedIfNotOfExpectedType()
        {
            //Arrange
            var queueService = new Mock<IAzureQueueService>();
            var message = new KeyValuePair<string, string>(typeof(WhenSignedAgreementEventTiggered).ToString(), "");

            //Act
            await HandleManageYourApprenticeshipsEvents.Run(message, queueService.Object, Mock.Of<ILogger>());

            //Assert
            queueService.Verify(s => s.SendMessage(It.IsAny<SignedAgreementEvent>(), QueueNames.SignedAgreement), Times.Never);
        }
    }
}