using System;
using System.Collections.Generic;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using TechTalk.SpecFlow;
using Reservation = SFA.DAS.Reservations.Domain.Reservations.Reservation;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests.Steps
{
    [Binding]
    public class ReservationCreatedSteps : StepsBase
    {
        public ReservationCreatedSteps(TestServiceProvider serviceProvider, TestData testData) : base(serviceProvider, testData)
        {
        }
        
        [Given(@"I have a reservation ready for creation")]
        public void GivenIHaveAReservationReadyForCreation()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            TestData.Reservation.Status = (short)ReservationStatus.Pending;

            dbContext.Reservations.Add(TestData.Reservation);
            dbContext.SaveChanges();
        }
        
        [When(@"a create reservation event is triggered")]
        public void WhenACreateReservationEventIsTriggered()
        {
            var permissionRepository = Services.GetService<IProviderPermissionRepository>();
            var mockPermissionRepository = Mock.Get(permissionRepository);

            var providerPermission = new ProviderPermission
                { AccountId = 1, AccountLegalEntityId = 1, CanCreateCohort = true, ProviderId = 1 };

            mockPermissionRepository.Setup(x => x.GetAllForAccountLegalEntity(TestData.AccountLegalEntity.AccountLegalEntityId))
                .Returns(new List<ProviderPermission> { providerPermission });

            var addNonLevyReservation = Services.GetService<IAddNonLevyReservationToReservationsIndexAction>();
            addNonLevyReservation.Execute(MapEntityReservationToReservation(TestData.Reservation));
        }
        
        [Then(@"the reservation search index should be updated with the new reservation")]
        public void ThenTheReservationSearchIndexShouldBeUpdatedWithTheNewReservation()
        {
            var indexRepository = Services.GetService<IReservationIndexRepository>();
            var mock = Mock.Get(indexRepository);

            mock.Verify(x => x.Add(It.IsAny<List<IndexedReservation>>()), Times.Once);
        }

        private Reservation MapEntityReservationToReservation(
            Domain.Entities.Reservation reservationToBeMapped)
        {
            var reservation = new Reservation
            {
                AccountId = reservationToBeMapped.AccountId,
                AccountLegalEntityId = reservationToBeMapped.AccountLegalEntityId,
                AccountLegalEntityName = reservationToBeMapped.AccountLegalEntityName,
                CourseId = reservationToBeMapped.CourseId,
                CourseName = TestData.Course.Title,
                CourseLevel = TestData.Course.Level,
                CreatedDate = reservationToBeMapped.CreatedDate,
                EmployerDeleted = false,
                EndDate = DateTime.UtcNow.AddMonths(6),
                Id = reservationToBeMapped.Id,
                ProviderId = reservationToBeMapped.ProviderId,
                StartDate = DateTime.UtcNow.AddMonths(1)
            };
            return reservation;
        }
    }
}
