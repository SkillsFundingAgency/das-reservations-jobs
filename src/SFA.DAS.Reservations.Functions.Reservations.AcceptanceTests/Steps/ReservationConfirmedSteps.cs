using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Reservations;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests.Steps
{
    [Binding]
    public class ReservationConfirmedSteps :StepsBase
    {
        public ReservationConfirmedSteps(TestServiceProvider serviceProvider, TestData testData) : base(serviceProvider, testData)
        {
        }
        
        [Given(@"I have a (.*) reservation")]
        public void GivenIHaveAReservation(string reservationStatus)
        {
            Enum.TryParse(reservationStatus, true, out ReservationStatus status);

            TestData.Reservation.Status = (short)status;

            var dbContext = Services.GetService<ReservationsDataContext>();

            dbContext.Reservations.Add(TestData.Reservation);
            dbContext.SaveChanges();
        }

        [Given(@"I have a reservation that doesnt exist")]
        public void GivenIHaveAReservationThatDoesntExist()
        {
            TestData.ReservationId = Guid.NewGuid();
        }

        [When(@"a confirm reservation event is triggered")]
        public void WhenAConfirmReservationEventIsTriggered()
        {
            TestData.DraftApprenticeshipCreatedEvent = new DraftApprenticeshipCreatedEvent(
                3459,
                76546,
                "4359805438",
                TestData.ReservationId,
                DateTime.UtcNow
                );

            var handler = Services.GetService<IConfirmReservationHandler>();
            handler.Handle(TestData.DraftApprenticeshipCreatedEvent).Wait();
        }
        
        [Then(@"the reservation status will be confirmed")]
        public void ThenTheReservationStatusWillBeConfirmed()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
            var reservation = dbContext.Reservations.Find(TestData.ReservationId);
            Assert.AreEqual(ReservationStatus.Confirmed,(ReservationStatus)reservation.Status);
            Assert.AreEqual(TestData.DraftApprenticeshipCreatedEvent.CreatedOn,reservation.ConfirmedDate);
            Assert.AreEqual(TestData.DraftApprenticeshipCreatedEvent.CohortId,reservation.CohortId);
            Assert.AreEqual(TestData.DraftApprenticeshipCreatedEvent.DraftApprenticeshipId,reservation.DraftApprenticeshipId);
            var reservationIndexRepository = Services.GetService<IReservationIndexRepository>();
            var mock = Mock.Get(reservationIndexRepository);
            mock.Verify(x=>x.SaveReservationStatus(
                TestData.ReservationId,ReservationStatus.Confirmed), Times.Once);
        }

        [Then(@"the reservation status will not be confirmed")]
        public void ThenTheReservationStatusWillNotBeConfirmed()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();
            var reservation = dbContext.Reservations.Find(TestData.ReservationId);
            Assert.IsNull(reservation);
            var reservationIndexRepository = Services.GetService<IReservationIndexRepository>();
            var mock = Mock.Get(reservationIndexRepository);
            mock.Verify(x=>x.SaveReservationStatus(TestData.ReservationId,ReservationStatus.Confirmed), Times.Never);
        }
    }
}
