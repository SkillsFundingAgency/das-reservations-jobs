using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
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
            _handler = new SignedLegalAgreementHandler(_service.Object);

        }

        [Test]
        public async Task Then_The_Service_Is_Called_To_Remove_The_Entity()
        {
            //Arrange
            var accountLegalEntityRemovedEvent = new SignedAgreementEvent { AccountId= 5, LegalEntityId = 56};

            //Act
            await _handler.Handler(accountLegalEntityRemovedEvent);

            //Assert
            _service.Verify(x => x.SignAgreementForAccountLegalEntity(It.Is<SignedAgreementEvent>(
                c => c.AccountId.Equals(accountLegalEntityRemovedEvent.AccountId) && 
                     c.LegalEntityId.Equals(accountLegalEntityRemovedEvent.LegalEntityId))));
        }
    }
}
