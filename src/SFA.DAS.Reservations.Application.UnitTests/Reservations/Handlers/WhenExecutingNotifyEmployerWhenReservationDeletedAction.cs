﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Application.UnitTests.Customisations;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Handlers
{
    public class WhenExecutingNotifyEmployerWhenReservationDeletedAction
    {
        [Test, MoqAutoData]
        public async Task And_No_ProviderId_Then_No_Further_Processing(
            ReservationDeletedEvent deletedEvent,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            NotifyEmployerWhenReservationDeletedAction action)
        {
            deletedEvent.ProviderId = null;

            await action.Execute(deletedEvent);

            mockAccountsService.Verify(service => service.GetAccountUsers(It.IsAny<long>()),
                Times.Never);
        }

        [Test, MoqAutoData]
        public async Task And_Not_Levy_Then_No_Further_Processing(
            ReservationDeletedEvent deletedEvent,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            NotifyEmployerWhenReservationDeletedAction action)
        {
            deletedEvent.CourseId = null;
            deletedEvent.StartDate = DateTime.MinValue;

            await action.Execute(deletedEvent);

            mockAccountsService.Verify(service => service.GetAccountUsers(It.IsAny<long>()),
                Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_Gets_All_Users_For_Account(
            ReservationDeletedEvent deletedEvent,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            NotifyEmployerWhenReservationDeletedAction action)
        {
            await action.Execute(deletedEvent);

            mockAccountsService.Verify(service => service.GetAccountUsers(deletedEvent.AccountId), 
                Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Sends_Message_For_Each_User(
            ReservationDeletedEvent deletedEvent,
            [ArrangeUsers] List<UserDetails> users,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<INotificationsService> mockNotificationsService,
            NotifyEmployerWhenReservationDeletedAction action)
        {
            mockAccountsService
                .Setup(service => service.GetAccountUsers(deletedEvent.AccountId))
                .ReturnsAsync(users);
                
            await action.Execute(deletedEvent);

            users.ForEach(user => 
                mockNotificationsService.Verify(service => 
                    service.SendNewReservationMessage(It.Is<NotificationMessage>(message => 
                        message.RecipientsAddress == user.Email)), Times.Once));
        }

        [Test, MoqAutoData]
        public async Task And_User_Not_Subscribed_Then_Skips(
            ReservationDeletedEvent deletedEvent,
            [ArrangeUsers] List<UserDetails> users,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<INotificationsService> mockNotificationsService,
            NotifyEmployerWhenReservationDeletedAction action)
        {
            users[0].CanReceiveNotifications = false;
            mockAccountsService
                .Setup(service => service.GetAccountUsers(deletedEvent.AccountId))
                .ReturnsAsync(users);

            await action.Execute(deletedEvent);

            mockNotificationsService.Verify(service => 
                service.SendNewReservationMessage(It.Is<NotificationMessage>(message => 
                    message.RecipientsAddress == users[0].Email)), Times.Never);
            users.Where(user => user.CanReceiveNotifications).ToList().ForEach(user => 
                mockNotificationsService.Verify(service => 
                    service.SendNewReservationMessage(It.Is<NotificationMessage>(message => 
                        message.RecipientsAddress == user.Email)), Times.Once));
        }

        [Test, MoqAutoData]
        public async Task And_User_Not_In_Owner_Role_Then_Skips(
            ReservationDeletedEvent deletedEvent,
            string otherRole,
            [ArrangeUsers] List<UserDetails> users,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<INotificationsService> mockNotificationsService,
            NotifyEmployerWhenReservationDeletedAction action)
        {
            users[0].Role = otherRole;
            mockAccountsService
                .Setup(service => service.GetAccountUsers(deletedEvent.AccountId))
                .ReturnsAsync(users);

            await action.Execute(deletedEvent);

            mockNotificationsService.Verify(service => 
                service.SendNewReservationMessage(It.Is<NotificationMessage>(message => 
                    message.RecipientsAddress == users[0].Email)), Times.Never);
            users.Where(user => user.Role == "Owner").ToList().ForEach(user => 
                mockNotificationsService.Verify(service => 
                    service.SendNewReservationMessage(It.Is<NotificationMessage>(message => 
                        message.RecipientsAddress == user.Email)), Times.Once));
        }

        [Test, MoqAutoData]
        public async Task And_User_Not_In_Transactor_Role_Then_Skips(
            ReservationDeletedEvent deletedEvent,
            string otherRole,
            [ArrangeUsers(Role = "Transactor")] List<UserDetails> users,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<INotificationsService> mockNotificationsService,
            NotifyEmployerWhenReservationDeletedAction action)
        {
            users[0].Role = otherRole;
            mockAccountsService
                .Setup(service => service.GetAccountUsers(deletedEvent.AccountId))
                .ReturnsAsync(users);

            await action.Execute(deletedEvent);

            mockNotificationsService.Verify(service => 
                service.SendNewReservationMessage(It.Is<NotificationMessage>(message => 
                    message.RecipientsAddress == users[0].Email)), Times.Never);
            users.Where(user => user.Role == "Transactor").ToList().ForEach(user => 
                mockNotificationsService.Verify(service => 
                    service.SendNewReservationMessage(It.Is<NotificationMessage>(message => 
                        message.RecipientsAddress == user.Email)), Times.Once));
        }

        [Test, MoqAutoData]
        public async Task Then_Sends_Message_With_Correct_Values(
            ReservationDeletedEvent deletedEvent,
            Dictionary<string, string> tokens,
            [ArrangeUsers] List<UserDetails> users,
            [Frozen] Mock<INotificationTokenBuilder> mockTokenBuilder,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<INotificationsService> mockNotificationsService,
            NotifyEmployerWhenReservationDeletedAction action)
        {
            mockAccountsService
                .Setup(service => service.GetAccountUsers(deletedEvent.AccountId))
                .ReturnsAsync(users);
            mockTokenBuilder
                .Setup(builder => builder.BuildReservationDeletedTokens(deletedEvent))
                .ReturnsAsync(tokens);
            
            await action.Execute(deletedEvent);

            mockNotificationsService.Verify(service =>
                service.SendNewReservationMessage(It.Is<NotificationMessage>(message =>
                    message.RecipientsAddress == users[0].Email &&
                    message.TemplateId == TemplateIds.ReservationCreated &&
                    message.Tokens == tokens))
                , Times.Once);
        }
    }
}