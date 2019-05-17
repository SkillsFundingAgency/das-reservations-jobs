using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Handlers
{
    public class WhenHandlingRemovingAccountLegalEntities
    {
        private Mock<IAccountLegalEntitiesService> _service;
        private RemoveLegalEntityHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _service = new Mock<IAccountLegalEntitiesService>();
            _handler = new RemoveLegalEntityHandler(_service.Object);

        }

        [Test]
        public async Task Then_The_Service_Is_Called_To_Remove_The_Entity()
        {
            //Arrange
            var accountLegalEntityRemovedEvent = new AccountLegalEntityRemovedEvent { AccountLegalEntityId = 5 };

            //Act
            await _handler.Handle(accountLegalEntityRemovedEvent);

            //Assert
            _service.Verify(x=>x.RemoveAccountLegalEntity(It.Is<AccountLegalEntityRemovedEvent>(
                c=>c.AccountLegalEntityId.Equals(accountLegalEntityRemovedEvent.AccountLegalEntityId))));
        }
    }
}
