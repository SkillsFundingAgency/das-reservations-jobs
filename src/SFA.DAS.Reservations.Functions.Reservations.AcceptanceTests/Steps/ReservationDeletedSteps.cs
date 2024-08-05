using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Notifications;
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
        
        [When(@"a delete reservation event is triggered by provider")]
        public void WhenADeleteReservationEventIsTriggeredByProvider()
        {
            var accountsService = Services.GetService<IAccountsService>();
            var mockAccountsService = Mock.Get(accountsService);

            mockAccountsService.Setup(x => x.GetAccountUsers(It.IsAny<long>())).ReturnsAsync(new List<TeamMember> { TestData.TeamMember });

            var notificationTokenBuilder = Services.GetService<INotificationTokenBuilder>();
            var mockNotificationTokenBuilder = Mock.Get(notificationTokenBuilder);

            mockNotificationTokenBuilder.Setup(x => x.BuildTokens(It.IsAny<INotificationEvent>()))
                .ReturnsAsync(new Dictionary<string, string>());

            var handler = Services.GetService<IReservationDeletedHandler>();
            handler.Handle(TestData.ReservationDeletedEvent).Wait();
        }

        [When(@"a delete reservation event for a reservation created by (.*) is triggered by employer")]
        public void WhenADeleteReservationEventForAReservationCreatedByIsTriggeredByEmployer(string source)
        {
            if (source.Equals("employer", StringComparison.CurrentCultureIgnoreCase))
            {
                TestData.ReservationDeletedEvent.ProviderId = null;
            }
            else if (source.Equals("provider", StringComparison.CurrentCultureIgnoreCase))
            {
                TestData.ReservationDeletedEvent.EmployerDeleted = true;
            }

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

        [Then(@"the employer should be notified of the deleted reservation")]
        public void ThenTheEmployerShouldBeNotifiedOfTheDeletedReservation()
        {
            var notificationsService = Services.GetService<INotificationsService>();
            var mock = Mock.Get(notificationsService);

            mock.Verify(x => x.SendEmail(It.IsAny<NotificationMessage>()), Times.Once);
        }
    }
}
