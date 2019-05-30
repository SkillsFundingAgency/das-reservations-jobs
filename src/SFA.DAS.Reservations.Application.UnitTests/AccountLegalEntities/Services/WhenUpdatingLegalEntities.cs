using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Services;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Services
{
    public class WhenUpdatingLegalEntities
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
        public async Task Then_The_Event_Is_Mapped_To_The_Entity_And_Update_Called_On_The_Repository()
        {
            //Arrange
            var signedAgreementEvent = new SignedAgreementEvent
            {
                AccountId = 9786,
                LegalEntityId = 543
            };

            //Act
            await _service.SignAgreementForAccountLegalEntity(signedAgreementEvent);

            //Assert
            _repository.Verify(x=>x.UpdateAgreementStatus(
                It.Is<AccountLegalEntity>(c=>c.AccountId.Equals(signedAgreementEvent.AccountId) 
                                             && c.LegalEntityId.Equals(signedAgreementEvent.LegalEntityId))));
        }
    }
}
