﻿using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Functions.LegalEntities.AcceptanceTests.Steps
{
    [Binding]
    public class LegalEntityCreatedSteps : StepsBase
    {
        public LegalEntityCreatedSteps(TestServiceProvider serviceProvider, TestData testData) : base(serviceProvider, testData)
        {
            
        }

        [Given(@"I have a legal entity that is new")]
        public void GivenIHaveALegalEntityThatIsNew()
        {
            TestData.AccountLegalEntity = new AccountLegalEntity
            {
                AccountId = TestData.AccountLegalEntity.AccountId,
                AccountLegalEntityId = TestData.AccountLegalEntity.AccountLegalEntityId + 1,
                AccountLegalEntityName = "Another legal entity",
                AgreementSigned =  true,
                AgreementType = AgreementType.NonLevyExpressionOfInterest,
                Id = Guid.NewGuid(),
                IsLevy = false,
                LegalEntityId = 123
            };

            var dbContext = Services.GetService<ReservationsDataContext>();
            var legalEntity = dbContext.AccountLegalEntities.SingleOrDefault(ale => 
                ale.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));

            Assert.IsNull(legalEntity);
        }
        
        [When(@"added legal entity event is triggered")]
        public void WhenAddedLegalEntityEventIsTriggered()
        {
            var handler = Services.GetService<IAddAccountLegalEntityHandler>();

            handler.Handle(new AddedLegalEntityEvent
            {
                AccountLegalEntityId = TestData.AccountLegalEntity.AccountLegalEntityId,
                AccountId = TestData.AccountLegalEntity.AccountId,
                LegalEntityId = TestData.AccountLegalEntity.LegalEntityId,
                Created = DateTime.Now,
                OrganisationName = TestData.AccountLegalEntity.AccountLegalEntityName
            }).Wait();
        }
        
        [Then(@"the legal entity should be available")]
        public void ThenTheLegalEntityShouldBeAvailable()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
            var legalEntity = dbContext.AccountLegalEntities.SingleOrDefault(ale =>
                ale.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));

            Assert.IsNotNull(legalEntity);
            legalEntity.Should().BeEquivalentTo(TestData.AccountLegalEntity, options => options
                .Excluding(ale => ale.Id)
                .Excluding(ale => ale.AgreementSigned)
                .Excluding(ale => ale.AgreementType));
        }
    }
}
