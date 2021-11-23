using System;
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

            try
            {
                handler.Handle(new SignedAgreementEvent
                {
                    AccountId = TestData.AccountLegalEntity.AccountId,
                    AgreementId = 123
                }).Wait();
            }
            catch (Exception e)
            {
                TestData.Exception = e;
            }
        }


        [Then(@"the legal entity should be signed")]
        public void ThenTheLegalEntityStateShouldBeSigned()
        {
            var dbContext = Services.GetService<TestReservationsDataContext>();
            var legalEntity = dbContext.AccountLegalEntities.SingleOrDefault(ale =>
                ale.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));

            Assert.IsNotNull(legalEntity);
            Assert.IsTrue(legalEntity.AgreementSigned);
        }

        [Then(@"an exception should be thrown")]
        public void ThenAnExceptionShouldBeThrown()
        {
            Assert.IsNotNull(TestData.Exception);
        }

    }
}
