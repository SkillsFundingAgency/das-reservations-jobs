﻿using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Handlers
{
    public class WhenHandlingAddingAccountLegalEntities
    {
        private Mock<IAccountLegalEntitiesService> _service;
        private AddAccountLegalEntityHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _service = new Mock<IAccountLegalEntitiesService>();
            _handler = new AddAccountLegalEntityHandler(_service.Object);

        }

        [Test]
        public async Task Then_The_Service_Is_Called_To_Add_The_Entity()
        {
            //Arrange
            var accountLegalEntityAddedEvent = new AddedLegalEntityEvent
            {
                AccountId = 65,
                LegalEntityId = 4434,
                OrganisationName = "Test",
                AccountLegalEntityId = 5                
            };

            //Act
            await _handler.Handle(accountLegalEntityAddedEvent);

            //Assert
            _service.Verify(x => x.AddAccountLegalEntity(It.Is<AddedLegalEntityEvent>(
                c => c.AccountLegalEntityId.Equals(accountLegalEntityAddedEvent.AccountLegalEntityId) &&
                     c.AccountId.Equals(accountLegalEntityAddedEvent.AccountId) && 
                     c.LegalEntityId.Equals(accountLegalEntityAddedEvent.LegalEntityId) &&
                     c.OrganisationName.Equals(accountLegalEntityAddedEvent.OrganisationName)
                )));
        }
    }
}
