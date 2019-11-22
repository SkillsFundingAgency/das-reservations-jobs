using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Functions.LegalEntities.AcceptanceTests.Steps
{
    [Binding]
    public class LegalEntityRemovedSteps : StepsBase
    {
        public LegalEntityRemovedSteps(TestServiceProvider serviceProvider, TestData testData) : base(serviceProvider, testData)
        {

        }

        [Given(@"I have an existing unsigned, non levy legal entity")]
        public void GivenIHaveAnExistingLegalEntity()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
            var legalEntity = dbContext.AccountLegalEntities.SingleOrDefault(ale =>
                ale.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));

            Assert.IsNotNull(legalEntity);
        }
        
        [When(@"removed legal entity event is triggered")]
        public void WhenRemovedLegalEntityEventIsTriggered()
        {
            var handler = Services.GetService<IRemoveLegalEntityHandler>();

            handler.Handle(new RemovedLegalEntityEvent
            {
                AccountLegalEntityId = TestData.AccountLegalEntity.AccountLegalEntityId
            }).Wait();
        }
        
        [Then(@"the legal entity should no longer be available")]
        public void ThenTheLegalEntityShouldNoLongerBeAvailable()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
            var legalEntity = dbContext.AccountLegalEntities.SingleOrDefault(ale =>
                ale.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));

            Assert.IsNull(legalEntity);
        }
    }
}
