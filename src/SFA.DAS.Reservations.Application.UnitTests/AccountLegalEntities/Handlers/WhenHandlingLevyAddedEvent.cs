using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Handlers
{
    public class WhenHandlingLevyAddedEvent
    {
        private Mock<IAccountLegalEntitiesService> _service;
        private LevyAddedToAccountHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _service = new Mock<IAccountLegalEntitiesService>();
            _handler = new LevyAddedToAccountHandler(_service.Object, Mock.Of<ILogger<LevyAddedToAccountHandler>>());
        }

        [Test]
        public async Task Then_The_Service_Is_Called_To_Update_The_Entity()
        {
            //Arrange
            var levyAddedToAccountEvent = new LevyAddedToAccount
            {
                AccountId= 5, 
                Amount = 100,
                Created = DateTime.Now
            };

            //Act
            await _handler.Handle(levyAddedToAccountEvent);

            //Assert
            _service.Verify(x => x.UpdateAccountLegalEntitiesToLevy(It.Is<LevyAddedToAccount>(
                c => c.AccountId.Equals(levyAddedToAccountEvent.AccountId) && 
                     c.Amount.Equals(levyAddedToAccountEvent.Amount) && 
                     c.Created.Equals(levyAddedToAccountEvent.Created))));
        }

        [Test]
        public async Task Then_Will_Not_Throw_Exception_If_Signing_Agreement_And_Database_Update_Fails()
        {
            //Arrange
            var levyAddedToAccountEvent = new LevyAddedToAccount
            {
                AccountId= 5, 
                Amount = 100,
                Created = DateTime.Now
            };

            _service.Setup(x => x.UpdateAccountLegalEntitiesToLevy(It.IsAny<LevyAddedToAccount>()))
                .ThrowsAsync(new DbUpdateException("Failed", (Exception)null));

            //Act + Assert
            await _handler.Handle(levyAddedToAccountEvent);
        }
    }
}
