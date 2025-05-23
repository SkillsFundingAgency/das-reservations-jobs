﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Notifications.Messages.Commands;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Application.UnitTests.Customisations;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Handlers
{
    public class WhenExecutingNotifyEmployerOfReservationEventAction
    {
        [Test, MoqAutoData]
        public async Task And_No_ProviderId_And_Then_No_Further_Processing(
            ReservationDeletedEvent deletedEvent,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<IMessageHandlerContext> mockMessageHandlerContext,
            NotifyEmployerOfReservationEventAction action)
        {
            deletedEvent.ProviderId = null;

            await action.Execute<ReservationDeletedNotificationEvent>(deletedEvent, mockMessageHandlerContext.Object);

            mockAccountsService.Verify(service => service.GetAccountUsers(It.IsAny<long>()),
                Times.Never);
        }

        [Test, MoqAutoData]
        public async Task And_Has_ProviderId_But_Deleted_By_Employer_Then_No_Further_Processing(
            ReservationDeletedEvent deletedEvent,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<IMessageHandlerContext> mockMessageHandlerContext,
            NotifyEmployerOfReservationEventAction action)
        {
            deletedEvent.EmployerDeleted = true;

            await action.Execute<ReservationDeletedNotificationEvent>(deletedEvent, mockMessageHandlerContext.Object);

            mockAccountsService.Verify(service => service.GetAccountUsers(It.IsAny<long>()),
                Times.Never);
        }

        [Test, MoqAutoData]
        public async Task And_Not_Levy_Then_No_Further_Processing(
            ReservationDeletedEvent deletedEvent,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<IMessageHandlerContext> mockMessageHandlerContext,
            NotifyEmployerOfReservationEventAction action)
        {
            deletedEvent.CourseId = null;
            deletedEvent.StartDate = DateTime.MinValue;

            await action.Execute<ReservationDeletedNotificationEvent>(deletedEvent, mockMessageHandlerContext.Object);

            mockAccountsService.Verify(service => service.GetAccountUsers(It.IsAny<long>()),
                Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_Gets_All_Users_For_Account(
            ReservationDeletedEvent deletedEvent,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<IMessageHandlerContext> mockMessageHandlerContext,
            NotifyEmployerOfReservationEventAction action)
        {
            await action.Execute<ReservationDeletedNotificationEvent>(deletedEvent, mockMessageHandlerContext.Object);

            mockAccountsService.Verify(service => service.GetAccountUsers(deletedEvent.AccountId), 
                Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Sends_Message_For_Each_User(
            ReservationDeletedEvent deletedEvent,
            [ArrangeUsers] List<TeamMember> users,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<IMessageHandlerContext> mockMessageHandlerContext,
            NotifyEmployerOfReservationEventAction action)
        {
            mockAccountsService
                .Setup(service => service.GetAccountUsers(deletedEvent.AccountId))
                .ReturnsAsync(users);
                
            await action.Execute<ReservationDeletedNotificationEvent>(deletedEvent, mockMessageHandlerContext.Object);

            users.ForEach(user =>
                mockMessageHandlerContext.Verify(service => 
                    service.Send(It.Is<SendEmailCommand>(message => 
                        message.RecipientsAddress == user.Email), It.IsAny<SendOptions>()), Times.Once));
        }

        [Test, MoqAutoData]
        public async Task And_User_Not_Subscribed_Then_Skips(
            ReservationDeletedEvent deletedEvent,
            [ArrangeUsers] List<TeamMember> users,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<IMessageHandlerContext> mockMessageHandlerContext,
            NotifyEmployerOfReservationEventAction action)
        {
            users[0].CanReceiveNotifications = false;
            mockAccountsService
                .Setup(service => service.GetAccountUsers(deletedEvent.AccountId))
                .ReturnsAsync(users);

            await action.Execute<ReservationDeletedNotificationEvent>(deletedEvent, mockMessageHandlerContext.Object);

            mockMessageHandlerContext.Verify(service => 
                service.Send(It.Is<SendEmailCommand>(message => 
                    message.RecipientsAddress == users[0].Email), It.IsAny<SendOptions>()), Times.Never);
            users.Where(user => user.CanReceiveNotifications).ToList().ForEach(user =>
                mockMessageHandlerContext.Verify(service => 
                    service.Send(It.Is<SendEmailCommand>(message => 
                        message.RecipientsAddress == user.Email), It.IsAny<SendOptions>()), Times.Once));
        }

        [Test, MoqAutoData]
        public async Task And_User_Not_In_Owner_Role_Then_Skips(
            ReservationDeletedEvent deletedEvent,
            string otherRole,
            [ArrangeUsers] List<TeamMember> users,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<IMessageHandlerContext> mockMessageHandlerContext,
            NotifyEmployerOfReservationEventAction action)
        {
            users[0].Role = otherRole;
            mockAccountsService
                .Setup(service => service.GetAccountUsers(deletedEvent.AccountId))
                .ReturnsAsync(users);

            await action.Execute<ReservationDeletedNotificationEvent>(deletedEvent, mockMessageHandlerContext.Object);

            mockMessageHandlerContext.Verify(service => 
                service.Send(It.Is<SendEmailCommand>(message => 
                    message.RecipientsAddress == users[0].Email), It.IsAny<SendOptions>()), Times.Never);
            users.Where(user => user.Role == "Owner").ToList().ForEach(user =>
                mockMessageHandlerContext.Verify(service => 
                    service.Send(It.Is<SendEmailCommand>(message => 
                        message.RecipientsAddress == user.Email), It.IsAny<SendOptions>()), Times.Once));
        }

        [Test, MoqAutoData]
        public async Task And_User_Not_In_Transactor_Role_Then_Skips(
            ReservationDeletedEvent deletedEvent,
            string otherRole,
            [ArrangeUsers(Role = "Transactor")] List<TeamMember> users,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<IMessageHandlerContext> mockMessageHandlerContext,
            NotifyEmployerOfReservationEventAction action)
        {
            users[0].Role = otherRole;
            mockAccountsService
                .Setup(service => service.GetAccountUsers(deletedEvent.AccountId))
                .ReturnsAsync(users);

            await action.Execute<ReservationDeletedNotificationEvent>(deletedEvent, mockMessageHandlerContext.Object);

            mockMessageHandlerContext.Verify(service => 
                service.Send(It.Is<SendEmailCommand>(message => 
                    message.RecipientsAddress == users[0].Email), It.IsAny<SendOptions>()), Times.Never);
            users.Where(user => user.Role == "Transactor").ToList().ForEach(user =>
                mockMessageHandlerContext.Verify(service => 
                    service.Send(It.Is<SendEmailCommand>(message => 
                        message.RecipientsAddress == user.Email), It.IsAny<SendOptions>()), Times.Once));
        }

        [Test, MoqAutoData]
        public async Task Then_Sends_Message_With_Correct_Values(
            ReservationDeletedEvent deletedEvent,
            Dictionary<string, string> tokens,
            [ArrangeUsers] List<TeamMember> users,
            [Frozen] Mock<INotificationTokenBuilder> mockTokenBuilder,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<IMessageHandlerContext> mockMessageHandlerContext,
            NotifyEmployerOfReservationEventAction action)
        {
            mockAccountsService
                .Setup(service => service.GetAccountUsers(deletedEvent.AccountId))
                .ReturnsAsync(users);
            mockTokenBuilder
                .Setup(builder => builder.BuildTokens(It.IsAny<INotificationEvent>()))
                .ReturnsAsync(tokens);
            
            await action.Execute<ReservationDeletedNotificationEvent>(deletedEvent, mockMessageHandlerContext.Object);

            mockMessageHandlerContext.Verify(service =>
                service.Send(It.Is<SendEmailCommand>(message =>
                    message.RecipientsAddress == users[0].Email &&
                    message.TemplateId == TemplateIds.ReservationDeleted &&
                    message.Tokens == tokens), It.IsAny<SendOptions>())
                , Times.Once);
        }

        [Test, MoqAutoData]
        public async Task And_Create_Notification_Then_Sends_Message_With_Create_Template(
            ReservationCreatedEvent createdEvent,
            Dictionary<string, string> tokens,
            [ArrangeUsers] List<TeamMember> users,
            [Frozen] Mock<INotificationTokenBuilder> mockTokenBuilder,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<IMessageHandlerContext> mockMessageHandlerContext,
            NotifyEmployerOfReservationEventAction action)
        {
            mockAccountsService
                .Setup(service => service.GetAccountUsers(createdEvent.AccountId))
                .ReturnsAsync(users);
            mockTokenBuilder
                .Setup(builder => builder.BuildTokens(It.IsAny<INotificationEvent>()))
                .ReturnsAsync(tokens);
            
            await action.Execute<ReservationCreatedNotificationEvent>(createdEvent, mockMessageHandlerContext.Object);

            mockMessageHandlerContext.Verify(service =>
                    service.Send(It.Is<SendEmailCommand>(message =>
                        message.TemplateId == TemplateIds.ReservationCreated ), It.IsAny<SendOptions>())
                , Times.Exactly(users.Count));
        }
    }
}