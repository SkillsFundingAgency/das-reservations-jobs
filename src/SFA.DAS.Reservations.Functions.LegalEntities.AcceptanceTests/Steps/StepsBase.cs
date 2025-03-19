using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Functions.LegalEntities.AcceptanceTests.Steps
{
    public class StepsBase(TestServiceProvider serviceProvider, TestData testData)
    {
        protected const long AccountId = 1;
        protected const long AccountLegalEntityId = 1;
        protected const uint ProviderId = 15214;
        protected Guid UserId;
        protected readonly TestData TestData = testData;
        protected readonly IServiceProvider Services = serviceProvider;
        private ReservationsDataContext dbContext;

        [AfterScenario]
        public void DisposeTestDatabaseData()
        {
            dbContext.Database.EnsureDeleted();
        }

        [BeforeScenario()]
        public void InitialiseTestDatabaseData()
        {
            TestData.Course = new Course
            {
                CourseId = "1",
                Level = 1,
                Title = "Tester"
            };

            TestData.AccountLegalEntity = new AccountLegalEntity
            {
                AccountId = AccountId,
                AccountLegalEntityId = AccountLegalEntityId,
                AccountLegalEntityName = "Test Corp",
                AgreementSigned = true
            };

            TestData.NonLevyAccount = new Account
            {
                Id = 1,
                Name = "Test Account",
                IsLevy = false
            };

            dbContext = Services.GetService<ReservationsDataContext>();

            if (dbContext.Apprenticeships.Find(TestData.Course.CourseId) == null)
            {
                dbContext.Apprenticeships.Add(TestData.Course);
            }

            var legalEntity = dbContext.AccountLegalEntities
                .SingleOrDefault(e => e.AccountLegalEntityId.Equals(TestData.AccountLegalEntity.AccountLegalEntityId));

            if (legalEntity == null)
            {
                dbContext.AccountLegalEntities.Add(TestData.AccountLegalEntity);

            }

            dbContext.SaveChanges();
        }
    }
}
