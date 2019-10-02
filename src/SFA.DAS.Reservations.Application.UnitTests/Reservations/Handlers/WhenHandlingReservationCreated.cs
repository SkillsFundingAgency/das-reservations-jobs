using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Application.UnitTests.Customisations;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Handlers
{
    public class WhenHandlingReservationCreated
    {
        [Test, MoqAutoData]
        public async Task And_No_ProviderId_Then_No_Further_Processing(
            ReservationCreatedEvent createdEvent,
            [Frozen] Mock<IProviderService> mockProviderService,
            ReservationCreatedHandler handler)
        {
            createdEvent.ProviderId = null;

            await handler.Handle(createdEvent);

            mockProviderService.Verify(service => service.GetDetails(It.IsAny<uint>()),
                Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_Gets_ProviderName(
            ReservationCreatedEvent createdEvent,
            [Frozen] Mock<IProviderService> mockProviderService,
            ReservationCreatedHandler handler)
        {
            await handler.Handle(createdEvent);

            mockProviderService.Verify(service => service.GetDetails(createdEvent.ProviderId.Value), 
                Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Gets_All_Users_For_Account(
            ReservationCreatedEvent createdEvent,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            ReservationCreatedHandler handler)
        {
            await handler.Handle(createdEvent);

            mockAccountsService.Verify(service => service.GetAccountUsers(createdEvent.AccountId), 
                Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Sends_Message_For_Each_User(
            ReservationCreatedEvent createdEvent,
            [UsersCanReceiveNotificationsAndWithRole(Role = "Owner")] List<UserDetails> users,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<INotificationsService> mockNotificationsService,
            ReservationCreatedHandler handler)
        {
            mockAccountsService
                .Setup(service => service.GetAccountUsers(createdEvent.AccountId))
                .ReturnsAsync(users);
                
            await handler.Handle(createdEvent);

            users.ForEach(user => 
                mockNotificationsService.Verify(service => 
                    service.SendNewReservationMessage(It.Is<ReservationCreatedMessage>(message => 
                        message.RecipientsAddress == user.Email)), Times.Once));
        }

        [Test, MoqAutoData]
        public async Task And_User_Not_Subscribed_Then_Skips(
            ReservationCreatedEvent createdEvent,
            [UsersCanReceiveNotificationsAndWithRole(Role = "Owner")] List<UserDetails> users,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<INotificationsService> mockNotificationsService,
            ReservationCreatedHandler handler)
        {
            users[0].CanReceiveNotifications = false;
            mockAccountsService
                .Setup(service => service.GetAccountUsers(createdEvent.AccountId))
                .ReturnsAsync(users);

            await handler.Handle(createdEvent);

            mockNotificationsService.Verify(service => 
                service.SendNewReservationMessage(It.Is<ReservationCreatedMessage>(message => 
                    message.RecipientsAddress == users[0].Email)), Times.Never);
            users.Where(user => user.CanReceiveNotifications).ToList().ForEach(user => 
                mockNotificationsService.Verify(service => 
                    service.SendNewReservationMessage(It.Is<ReservationCreatedMessage>(message => 
                        message.RecipientsAddress == user.Email)), Times.Once));
        }

        [Test, MoqAutoData]
        public async Task And_User_Not_In_Owner_Role_Then_Skips(
            ReservationCreatedEvent createdEvent,
            string otherRole,
            [UsersCanReceiveNotificationsAndWithRole(Role = "Owner")] List<UserDetails> users,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<INotificationsService> mockNotificationsService,
            ReservationCreatedHandler handler)
        {
            users[0].Role = otherRole;
            mockAccountsService
                .Setup(service => service.GetAccountUsers(createdEvent.AccountId))
                .ReturnsAsync(users);

            await handler.Handle(createdEvent);

            mockNotificationsService.Verify(service => 
                service.SendNewReservationMessage(It.Is<ReservationCreatedMessage>(message => 
                    message.RecipientsAddress == users[0].Email)), Times.Never);
            users.Where(user => user.Role == "Owner").ToList().ForEach(user => 
                mockNotificationsService.Verify(service => 
                    service.SendNewReservationMessage(It.Is<ReservationCreatedMessage>(message => 
                        message.RecipientsAddress == user.Email)), Times.Once));
        }

        [Test, MoqAutoData]
        public async Task And_User_Not_In_Transactor_Role_Then_Skips(
            ReservationCreatedEvent createdEvent,
            string otherRole,
            [UsersCanReceiveNotificationsAndWithRole(Role = "Transactor")] List<UserDetails> users,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<INotificationsService> mockNotificationsService,
            ReservationCreatedHandler handler)
        {
            users[0].Role = otherRole;
            mockAccountsService
                .Setup(service => service.GetAccountUsers(createdEvent.AccountId))
                .ReturnsAsync(users);

            await handler.Handle(createdEvent);

            mockNotificationsService.Verify(service => 
                service.SendNewReservationMessage(It.Is<ReservationCreatedMessage>(message => 
                    message.RecipientsAddress == users[0].Email)), Times.Never);
            users.Where(user => user.Role == "Transactor").ToList().ForEach(user => 
                mockNotificationsService.Verify(service => 
                    service.SendNewReservationMessage(It.Is<ReservationCreatedMessage>(message => 
                        message.RecipientsAddress == user.Email)), Times.Once));
        }

        // then gets template id from config
    }
}