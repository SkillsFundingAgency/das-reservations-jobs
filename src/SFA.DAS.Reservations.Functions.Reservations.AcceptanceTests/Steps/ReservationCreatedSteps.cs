using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;
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
            TestData.Reservation.Status = (short)ReservationStatus.Pending;
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

            var accountsService = Services.GetService<IAccountsService>();
            var mockAccountsService = Mock.Get(accountsService);

            var userDetails = new UserDetails{CanReceiveNotifications = true, Email = "", Name = "", Role = "Owner", Status = 1, UserRef = ""};

            mockAccountsService.Setup(x => x.GetAccountUsers(It.IsAny<long>())).ReturnsAsync(new List<UserDetails> {userDetails});

            var notificationTokenBuilder = Services.GetService<INotificationTokenBuilder>();
            var mockNotificationTokenBuilder = Mock.Get(notificationTokenBuilder);

            mockNotificationTokenBuilder.Setup(x => x.BuildTokens(It.IsAny<INotificationEvent>()))
                .ReturnsAsync(new Dictionary<string, string>());

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
    }
}
