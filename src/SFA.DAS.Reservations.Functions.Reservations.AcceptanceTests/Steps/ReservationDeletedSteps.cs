using System;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Reservations;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests.Steps
{
    [Binding]
    public class ReservationDeletedSteps : StepsBase
    {
        public ReservationDeletedSteps(TestServiceProvider serviceProvider, TestData testData) : base(serviceProvider, testData)
        {
        }

        private Domain.Entities.Reservation _reservation = new Domain.Entities.Reservation();

        [Given(@"I have a reservation ready for deletion")]
        public void GivenIHaveAReservationReadyForDeletion()
        {
            TestData.ReservationId = Guid.NewGuid();

            var dbContext = Services.GetService<ReservationsDataContext>();

            _reservation = new Domain.Entities.Reservation
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
                UserId = Guid.NewGuid()
            };

            dbContext.Reservations.Add(_reservation);
            dbContext.SaveChanges();
        }
        
        [When(@"a delete reservation event is triggered")]
        public void WhenADeleteReservationEventIsTriggered()
        {
            var reservationService = Services.GetService<IReservationService>();
            reservationService.UpdateReservationStatus(_reservation.Id, (ReservationStatus) _reservation.Status);
        }
        
        [Then(@"the reservation search index should be updated with the deleted reservation removed")]
        public void ThenTheReservationSearchIndexShouldBeUpdatedWithTheDeletedReservationRemoved()
        {
            var reservationIndexRepository = Services.GetService<IReservationIndexRepository>();
            var mock = Mock.Get(reservationIndexRepository);
            mock.Verify(x => x.SaveReservationStatus(TestData.ReservationId, ReservationStatus.Deleted), Times.Once);
        }
    }
}
