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
        
        [Given(@"I have a reservation ready for deletion")]
        public void GivenIHaveAReservationReadyForDeletion()
        {
            TestData.Reservation.Status = (short)ReservationStatus.Deleted;

            var dbContext = Services.GetService<ReservationsDataContext>();

            dbContext.Reservations.Add(TestData.Reservation);
            dbContext.SaveChanges();
        }
        
        [When(@"a delete reservation event is triggered")]
        public void WhenADeleteReservationEventIsTriggered()
        {
            var handler = Services.GetService<IReservationDeletedHandler>();
            handler.Handle(TestData.ReservationDeletedEvent).Wait();
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
