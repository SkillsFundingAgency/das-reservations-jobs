using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Handlers
{
    public class WhenHandlingAddingAccountLegalEntities
    {
        private Mock<IAccountLegalEntitiesService> _service;
        private AddAccountLegalEntityHandler _handler;
        private Mock<IValidator<AddedLegalEntityEvent>> _validator;

        [SetUp]
        public void Arrange()
        {
            _service = new Mock<IAccountLegalEntitiesService>();
            _validator = new Mock<IValidator<AddedLegalEntityEvent>>();
            _handler = new AddAccountLegalEntityHandler(
                _service.Object, 
                _validator.Object, 
                Mock.Of<ILogger<AddAccountLegalEntityHandler>>());
        }

        [Test]
        public async Task Then_The_Service_Is_Called_To_Add_The_Entity()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<AddedLegalEntityEvent>()))
                .ReturnsAsync(new ValidationResult());

            var accountLegalEntityAddedEvent = new AddedLegalEntityEvent
            {
                AccountId = 65,
                LegalEntityId = 4434,
                OrganisationName = "Test",
                AccountLegalEntityId = 5                
            };

            //Act
            await _handler.Handle(accountLegalEntityAddedEvent);

            //Assert
            _service.Verify(x => x.AddAccountLegalEntity(It.Is<AddedLegalEntityEvent>(
                c => c.AccountLegalEntityId.Equals(accountLegalEntityAddedEvent.AccountLegalEntityId) &&
                     c.AccountId.Equals(accountLegalEntityAddedEvent.AccountId) && 
                     c.LegalEntityId.Equals(accountLegalEntityAddedEvent.LegalEntityId) &&
                     c.OrganisationName.Equals(accountLegalEntityAddedEvent.OrganisationName)
                )));
        }

        [Test]
        public async Task Then_The_Service_Is_Not_Called_If_Validation_Fails()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<AddedLegalEntityEvent>()))
                .ReturnsAsync(new ValidationResult{ValidationDictionary = new Dictionary<string, string> {{"test", "error"}}});

            var accountLegalEntityAddedEvent = new AddedLegalEntityEvent
            {
                AccountId = 65,
                LegalEntityId = 4434,
                OrganisationName = "Test",
                AccountLegalEntityId = 5
            };

            //Act
            await _handler.Handle(accountLegalEntityAddedEvent);

            //Assert
            _service.Verify(x => x.AddAccountLegalEntity(It.IsAny<AddedLegalEntityEvent>()), Times.Never());
               
        }
    }
}
