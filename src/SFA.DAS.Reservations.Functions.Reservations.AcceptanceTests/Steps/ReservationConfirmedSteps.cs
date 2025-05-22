using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Reservations;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests.Steps;

[Binding]
public class ReservationConfirmedSteps(TestServiceProvider serviceProvider, TestData testData)
    : StepsBase(serviceProvider, testData)
{
    [Given(@"I have a (.*) reservation")]
    public void GivenIHaveAReservation(ReservationStatus reservationStatus)
    {
        TestData.Reservation.Status = (short)reservationStatus;

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

        ((ReservationStatus)reservation.Status).Should().Be(ReservationStatus.Confirmed);
        reservation.ConfirmedDate.Should().Be(TestData.DraftApprenticeshipCreatedEvent.CreatedOn);
        reservation.CohortId.Should().Be(TestData.DraftApprenticeshipCreatedEvent.CohortId);
        reservation.DraftApprenticeshipId.Should().Be(TestData.DraftApprenticeshipCreatedEvent.DraftApprenticeshipId);

        var reservationIndexRepository = Services.GetService<IElasticReservationIndexRepository>();
        var mock = Mock.Get(reservationIndexRepository);

        mock.Verify(x => x.SaveReservationStatus(
            TestData.ReservationId, ReservationStatus.Confirmed), Times.Once);
    }

    [Then(@"the reservation status will not be confirmed")]
    public void ThenTheReservationStatusWillNotBeConfirmed()
    {
        var dbContext = Services.GetService<ReservationsDataContext>();
        var reservation = dbContext.Reservations.Find(TestData.ReservationId) ?? new Domain.Entities.Reservation();

        ((ReservationStatus)reservation.Status).Should().NotBe(ReservationStatus.Confirmed);
    }
}