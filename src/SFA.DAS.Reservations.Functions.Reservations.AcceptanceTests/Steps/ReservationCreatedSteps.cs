using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NServiceBus;
using SFA.DAS.Notifications.Messages.Commands;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests.Steps
{
    [Binding]
    public class ReservationCreatedSteps(TestServiceProvider serviceProvider, TestData testData)
        : StepsBase(serviceProvider, testData)
    {
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

            mockAccountsService.Setup(x => x.GetAccountUsers(It.IsAny<long>())).ReturnsAsync(new List<TeamMember> { TestData.TeamMember });

            var notificationTokenBuilder = Services.GetService<INotificationTokenBuilder>();
            var mockNotificationTokenBuilder = Mock.Get(notificationTokenBuilder);

            mockNotificationTokenBuilder.Setup(x => x.BuildTokens(It.IsAny<INotificationEvent>()))
                .ReturnsAsync(new Dictionary<string, string>());

            var handler = Services.GetService<IReservationCreatedHandler>();
            TestData.MessageHandlerContext = new Mock<IMessageHandlerContext>();
            handler.Handle(TestData.ReservationCreatedEvent, TestData.MessageHandlerContext.Object).Wait();
        }

        [When(@"a create reservation event is triggered by employer")]
        public void WhenACreateReservationEventIsTriggeredByEmployer()
        {
            TestData.ReservationCreatedEvent.ProviderId = null;

            var handler = Services.GetService<IReservationCreatedHandler>();
            TestData.MessageHandlerContext = new Mock<IMessageHandlerContext>();
            handler.Handle(TestData.ReservationCreatedEvent, TestData.MessageHandlerContext.Object).Wait();
        }

        [When(@"a create reservation event is triggered for a levy employer")]
        public void WhenACreatedReservationEventIsTriggeredForALevyEmployer()
        {
            TestData.ReservationCreatedEvent.CourseId = null;
            TestData.ReservationCreatedEvent.StartDate = DateTime.MinValue;

            var handler = Services.GetService<IReservationCreatedHandler>();
            TestData.MessageHandlerContext = new Mock<IMessageHandlerContext>();
            handler.Handle(TestData.ReservationCreatedEvent, TestData.MessageHandlerContext.Object).Wait();
        }

        [Then(@"the reservation search index should be updated with the new reservation")]
        public void ThenTheReservationSearchIndexShouldBeUpdatedWithTheNewReservation()
        {
            var azureSearchIndexRepository = Services.GetService<IAzureSearchReservationIndexRepository>();
            var azSearchMock = Mock.Get(azureSearchIndexRepository);

            azSearchMock.Verify(x => x.Add(It.IsAny<List<IndexedReservation>>(), It.IsAny<string>()), Times.Once);
        }

        [Then(@"the employer should be notified of the created reservation")]
        public void ThenTheEmployerShouldBeNotifiedOfTheCreatedReservation()
        {
            TestData.MessageHandlerContext.Verify(x => x.Send(It.IsAny<SendEmailCommand>(), It.IsAny<SendOptions>()), Times.Once);
        }

        [Then(@"the employer should not be notified of the (.*) reservation")]
        public void ThenTheEmployerShouldNotBeNotifiedOfTheReservation(string reservationStatus)
        {
            TestData.MessageHandlerContext.Verify(x => x.Send(It.IsAny<SendEmailCommand>(), It.IsAny<SendOptions>()), Times.Never);
        }
    }
}
