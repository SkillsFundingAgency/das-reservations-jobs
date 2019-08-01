using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Testing.AutoFixture;

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
            await _handler.Handle(accountLegalEntityRemovedEvent);

            //Assert
            _service.Verify(x => x.SignAgreementForAccountLegalEntity(It.Is<SignedAgreementEvent>(
                c => c.AccountId.Equals(accountLegalEntityRemovedEvent.AccountId) && 
                     c.LegalEntityId.Equals(accountLegalEntityRemovedEvent.LegalEntityId))));
        }

        [Test, MoqAutoData]
        public async Task AndLevyAddedToAccount_ThenServiceCalledCorrectly(
            [Frozen] Mock<IAccountLegalEntitiesService> service,
            LevyAddedToAccountHandler handler,
            LevyAddedToAccount levyAddedToAccountEvent)
        {
            //Act
            await handler.Handle(levyAddedToAccountEvent);

            //Assert
            service.Verify(x => x.UpdateAccountLegalEntitiesToLevy(levyAddedToAccountEvent));
        }
    }
}
