using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests.Steps
{
    public class StepsBase
    {
        protected const long AccountId = 1;
        protected const long AccountLegalEntityId = 1;
        protected const uint ProviderId = 15214;
        protected Guid UserId;
        protected readonly TestData TestData;
        protected readonly IServiceProvider Services;
        private ReservationsDataContext dbContext;
        public StepsBase(TestServiceProvider serviceProvider, TestData testData)
        {
            Services = serviceProvider;
            TestData = testData;
        }


        [BeforeScenario()]
        public void InitialiseTestData()
        {
            InitialiseTestDatabaseData();
            InitialiseTestDataReservation();
            InitialiseReservationCreatedEvent();
            InitialiseReservationDeletedEvent();
            InitialiseProviderPermission();
            InitialiseUserDetails();
        }


        [AfterScenario]
        public void DisposeTestDatabaseData()
        {
            dbContext.Database.EnsureDeleted();
        }

        private void InitialiseTestDatabaseData()
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

                dbContext.SaveChanges();
            }
        }

        private void InitialiseTestDataReservation()
        {
            TestData.ReservationId = Guid.NewGuid();

            TestData.Reservation = new Domain.Entities.Reservation
            {
                AccountId = 1,
                AccountLegalEntityId = TestData.AccountLegalEntity.AccountLegalEntityId,
                AccountLegalEntityName = TestData.AccountLegalEntity.AccountLegalEntityName,
                CourseId = TestData.Course.CourseId,
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(2),
                IsLevyAccount = false,
                Status = (short)ReservationStatus.Deleted,
                StartDate = DateTime.UtcNow.AddMonths(1),
                Id = TestData.ReservationId,
                UserId = Guid.NewGuid(),
                ProviderId = 1
            };
        }

        private void InitialiseReservationCreatedEvent()
        {
            TestData.ReservationCreatedEvent = new ReservationCreatedEvent(
                TestData.Reservation.Id,
                TestData.Reservation.AccountId,
                TestData.Reservation.AccountLegalEntityId,
                TestData.Reservation.AccountLegalEntityName,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMonths(3),
                TestData.Reservation.CreatedDate,
                TestData.Course.CourseId,
                TestData.Course.Title,
                TestData.Course.Level.ToString(),
                TestData.Reservation.ProviderId);
        }

        private void InitialiseReservationDeletedEvent()
        {
            TestData.ReservationDeletedEvent = new ReservationDeletedEvent(
                TestData.Reservation.Id,
                TestData.Reservation.AccountId,
                TestData.Reservation.AccountLegalEntityId,
                TestData.Reservation.AccountLegalEntityName,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMonths(3),
                TestData.Reservation.CreatedDate,
                TestData.Reservation.CourseId,
                TestData.Course.Title,
                TestData.Course.Level.ToString(),
                TestData.Reservation.ProviderId,
                false
            );
        }

        private void InitialiseProviderPermission()
        {
            TestData.ProviderPermission = new ProviderPermission
            { AccountId = 1, AccountLegalEntityId = 1, CanCreateCohort = true, ProviderId = 1 };
        }

        private void InitialiseUserDetails()
        {
            TestData.UserDetails = new UserDetails
            { CanReceiveNotifications = true, Email = "", Name = "", Role = "Owner", Status = 1, UserRef = "" };
        }
    }
}
