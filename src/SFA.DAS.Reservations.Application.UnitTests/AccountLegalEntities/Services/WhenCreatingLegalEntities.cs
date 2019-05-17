using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Services;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Services
{
    public class WhenCreatingLegalEntities
    {
        private AccountLegalEntitiesService _service;
        private Mock<IAccountLegalEntityRepository> _repository;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IAccountLegalEntityRepository>();
            _service = new AccountLegalEntitiesService(_repository.Object);
        }

        [Test]
        public async Task Then_The_Event_Is_Mapped_To_The_Entity_And_Repository_Called()
        {
            //Arrange
            var accountLegalEntityAddedEvent = new AccountLegalEntityAddedEvent
            {
                AccountLegalEntityId = 23432,
                AccountId = 9786,
                LegalEntityId = 543,
                OrganisationName = "Test Account"
            };

            //Act
            await _service.AddAccountLegalEntity(accountLegalEntityAddedEvent);

            //Assert
            _repository.Verify(x=>x.Add(It.Is<AccountLegalEntity>(c=>
                !c.AgreementSigned &&
                c.AccountLegalEntityId.Equals(accountLegalEntityAddedEvent.AccountLegalEntityId) &&
                c.AccountId.Equals(accountLegalEntityAddedEvent.AccountId) &&
                c.AccountLegalEntityName.Equals(accountLegalEntityAddedEvent.OrganisationName) &&
                c.LegalEntityId.Equals(accountLegalEntityAddedEvent.LegalEntityId)
                )), Times.Once);
        }
    }
}
