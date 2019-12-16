using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Handlers
{
    public class WhenHandlingUpdatingAccountLegalEntities
    {
        private Mock<IAccountLegalEntitiesService> _service;
        private SignedLegalAgreementHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _service = new Mock<IAccountLegalEntitiesService>();
            _handler = new SignedLegalAgreementHandler(_service.Object, Mock.Of<ILogger<SignedLegalAgreementHandler>>());
        }

        [Test]
        public async Task Then_The_Service_Is_Called_To_Update_The_Entity()
        {
            //Arrange
            var signedAgreementEvent = new SignedAgreementEvent
            {
                AccountId= 5, 
                LegalEntityId = 56
            };

            //Act
            await _handler.Handle(signedAgreementEvent);

            //Assert
            _service.Verify(x => x.SignAgreementForAccountLegalEntity(It.Is<SignedAgreementEvent>(
                c => c.AccountId.Equals(signedAgreementEvent.AccountId) && 
                     c.LegalEntityId.Equals(signedAgreementEvent.LegalEntityId))));
        }

        [Test]
        public void Then_Will_Not_Throw_Exception_If_Signing_Agreement_And_Database_Update_Fails()
        {
            //Arrange
            var signedAgreementEvent = new SignedAgreementEvent
            {
                AccountId= 5, 
                LegalEntityId = 56
            };

            _service.Setup(x => x.SignAgreementForAccountLegalEntity(It.IsAny<SignedAgreementEvent>()))
                .ThrowsAsync(new DbUpdateException("Failed", (Exception)null));

            //Act + Assert
            Assert.ThrowsAsync<DbUpdateException>(() => _handler.Handle(signedAgreementEvent));
        }
    }
}
