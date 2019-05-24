using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Services;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Services
{
    public class WhenRemovingALegalEntity
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
        public async Task Then_The_Event_Is_Mapped_And_The_Repository_Called()
        {
            //Arrange
            var accountLegalEntityRemovedEvent = new RemovedLegalEntityEvent();

            //Act
            await _service.RemoveAccountLegalEntity(accountLegalEntityRemovedEvent);

            //Assert
            _repository.Verify(x=>x.Remove(It.Is<AccountLegalEntity>(c=>c.AccountLegalEntityId.Equals(accountLegalEntityRemovedEvent.AccountLegalEntityId))), Times.Once);
        }
    }
}
