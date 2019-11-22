using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Functions.LegalEntities.AcceptanceTests.Steps
{
    [Binding]
    public class LegalEntityUpdatedSteps : StepsBase
    {
        public LegalEntityUpdatedSteps(TestServiceProvider serviceProvider, TestData testData) : base(serviceProvider,
            testData)
        {

        }

        [When(@"signed agreement event is triggered")]
        public void WhenSignedAgreementEventIsTriggered()
        {
            var handler = Services.GetService<ISignedLegalAgreementHandler>();

            handler.Handle(new SignedAgreementEvent
            {
                AccountId = TestData.AccountLegalEntity.AccountId,
                AgreementId = 123,
                AgreementType = AgreementType.NonLevyExpressionOfInterest
            }).Wait();
        }

        [When(@"levy added event is triggered")]
        public void WhenLevyAddedEventIsTriggered()
        {
            var handler = Services.GetService<ILevyAddedToAccountHandler>();

            handler.Handle(new LevyAddedToAccount
            {
                AccountId = TestData.AccountLegalEntity.AccountId
            }).Wait();
        }

        [Then(@"the legal entity should be signed")]
        public void ThenTheLegalEntityStateShouldBeSigned()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
            var legalEntity = dbContext.AccountLegalEntities.SingleOrDefault(ale =>
                ale.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));

            Assert.IsNotNull(legalEntity);
            Assert.IsTrue(legalEntity.AgreementSigned);
        }

        [Then(@"the legal entity should be marked as a levy entity")]
        public void ThenTheLegalEntityShouldBeMarkedAsALevyEntity()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
            var legalEntity = dbContext.AccountLegalEntities.SingleOrDefault(ale =>
                ale.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));

            Assert.IsNotNull(legalEntity);
            Assert.IsTrue(legalEntity.IsLevy);

        }
    }
}
