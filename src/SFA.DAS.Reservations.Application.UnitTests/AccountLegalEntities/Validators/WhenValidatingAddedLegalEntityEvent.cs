using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Validators;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Validators
{
    public class WhenValidatingAddedLegalEntityEvent
    {
        [Test]
        public async Task ThenIfValidWillPassValidation()
        {
            //Arrange
            var validator = new AddAccountLegalEntityValidator();
            var @event = new AddedLegalEntityEvent
            {
                AccountId = 12,
                AccountLegalEntityId = 1,
                LegalEntityId = 2,
                OrganisationName = "test"
            };

            //Act
            var result = await validator.ValidateAsync(@event);

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public async Task ThenIfHasNoAccountIdWillFailValidation()
        {
            //Arrange
            var validator = new AddAccountLegalEntityValidator();
            var @event = new AddedLegalEntityEvent
            {
                AccountLegalEntityId = 1,
                LegalEntityId = 2,
                OrganisationName = "test"
            };

            //Act
            var result = await validator.ValidateAsync(@event);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(AddedLegalEntityEvent.AccountId)));
        }

        [Test]
        public async Task ThenIfHasNoAccountLegalEntityIdWillFailValidation()
        {
            //Arrange
            var validator = new AddAccountLegalEntityValidator();
            var @event = new AddedLegalEntityEvent
            {
                AccountId = 12,
                LegalEntityId = 2,
                OrganisationName = "test"
            };

            //Act
            var result = await validator.ValidateAsync(@event);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(AddedLegalEntityEvent.AccountLegalEntityId)));
        }

        [Test]
        public async Task ThenIfHasNoLegalEntityIdWillFailValidation()
        {
            //Arrange
            var validator = new AddAccountLegalEntityValidator();
            var @event = new AddedLegalEntityEvent
            {
                AccountId = 12,
                AccountLegalEntityId = 1,
                OrganisationName = "test"
            };

            //Act
            var result = await validator.ValidateAsync(@event);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(AddedLegalEntityEvent.LegalEntityId)));
        }

        [Test]
        public async Task ThenIfHasNoOrganisationNameWillFailValidation()
        {
            //Arrange
            var validator = new AddAccountLegalEntityValidator();
            var @event = new AddedLegalEntityEvent
            {
                AccountId = 12,
                AccountLegalEntityId = 1,
                LegalEntityId = 2
            };

            //Act
            var result = await validator.ValidateAsync(@event);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(AddedLegalEntityEvent.OrganisationName)));
        }
    }
}
