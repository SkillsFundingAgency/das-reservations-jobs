using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
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
        public async Task Then_The_Service_Is_Called_To_Update_The_Entity()
        {
            //Arrange
            var signedAgreementEvent = new SignedAgreementEvent
            {
                AccountId= 5, 
                LegalEntityId = 56, 
                AgreementType = AgreementType.NoneLevyExpressionOfInterest
            };

            //Act
            await _handler.Handle(signedAgreementEvent);

            //Assert
            _service.Verify(x => x.SignAgreementForAccountLegalEntity(It.Is<SignedAgreementEvent>(
                c => c.AccountId.Equals(signedAgreementEvent.AccountId) && 
                     c.LegalEntityId.Equals(signedAgreementEvent.LegalEntityId) && 
                     c.AgreementType.Equals(signedAgreementEvent.AgreementType))));
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
