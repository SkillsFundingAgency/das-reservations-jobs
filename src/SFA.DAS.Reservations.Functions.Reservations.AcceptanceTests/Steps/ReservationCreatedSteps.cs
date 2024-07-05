using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using TechTalk.SpecFlow;

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
            TestData.Reservation.Status = (short)ReservationStatus.Pending;
        }
        
        [When(@"a create reservation event is triggered by provider")]
        public void WhenACreateReservationEventIsTriggeredByProvider()
        {
            var permissionRepository = Services.GetService<IProviderPermissionRepository>();
            var mockPermissionRepository = Mock.Get(permissionRepository);

            mockPermissionRepository.Setup(x => x.GetAllForAccountLegalEntity(TestData.AccountLegalEntity.AccountLegalEntityId))
                .Returns(new List<ProviderPermission> { TestData.ProviderPermission });

            var accountsService = Services.GetService<IAccountsService>();
            var mockAccountsService = Mock.Get(accountsService);

            mockAccountsService.Setup(x => x.GetAccountUsers(It.IsAny<long>())).ReturnsAsync(new List<TeamMember> {TestData.TeamMember});

            var notificationTokenBuilder = Services.GetService<INotificationTokenBuilder>();
            var mockNotificationTokenBuilder = Mock.Get(notificationTokenBuilder);

            mockNotificationTokenBuilder.Setup(x => x.BuildTokens(It.IsAny<INotificationEvent>()))
                .ReturnsAsync(new Dictionary<string, string>());

            var handler = Services.GetService<IReservationCreatedHandler>();
            handler.Handle(TestData.ReservationCreatedEvent).Wait();
        }

        [When(@"a create reservation event is triggered by employer")]
        public void WhenACreateReservationEventIsTriggeredByEmployer()
        {
            TestData.ReservationCreatedEvent.ProviderId = null;

            var handler = Services.GetService<IReservationCreatedHandler>();
            handler.Handle(TestData.ReservationCreatedEvent).Wait();
        }

        [When(@"a create reservation event is triggered for a levy employer")]
        public void WhenACreatedReservationEventIsTriggeredForALevyEmployer()
        {
            TestData.ReservationCreatedEvent.CourseId = null;
            TestData.ReservationCreatedEvent.StartDate = DateTime.MinValue;

            var handler = Services.GetService<IReservationCreatedHandler>();
            handler.Handle(TestData.ReservationCreatedEvent).Wait();
        }

        [Then(@"the reservation search index should be updated with the new reservation")]
        public void ThenTheReservationSearchIndexShouldBeUpdatedWithTheNewReservation()
        {
            var indexRepository = Services.GetService<IReservationIndexRepository>();
            var mock = Mock.Get(indexRepository);

            mock.Verify(x => x.Add(It.IsAny<List<IndexedReservation>>()), Times.Once);
        }

        [Then(@"the employer should be notified of the created reservation")]
        public void ThenTheEmployerShouldBeNotifiedOfTheCreatedReservation()
        {
            var notificationsService = Services.GetService<INotificationsService>();
            var mock = Mock.Get(notificationsService);

            mock.Verify(x => x.SendNewReservationMessage(It.IsAny<NotificationMessage>()),Times.Once);
        }

        [Then(@"the employer should not be notified of the (.*) reservation")]
        public void ThenTheEmployerShouldNotBeNotifiedOfTheReservation(string reservationStatus)
        {
            var notificationsService = Services.GetService<INotificationsService>();
            var mock = Mock.Get(notificationsService);

            mock.Verify(x => x.SendNewReservationMessage(It.IsAny<NotificationMessage>()), Times.Never);
        }
    }
}
