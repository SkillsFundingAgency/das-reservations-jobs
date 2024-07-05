using System.Threading.Tasks;
using FluentAssertions;
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
            result.IsValid().Should().BeTrue();
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
            result.IsValid().Should().BeFalse();
            result.ValidationDictionary.ContainsKey(nameof(AddedLegalEntityEvent.AccountId)).Should().BeTrue();
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
            result.IsValid().Should().BeFalse();
            result.ValidationDictionary.ContainsKey(nameof(AddedLegalEntityEvent.AccountLegalEntityId)).Should().BeTrue();
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
            result.IsValid().Should().BeFalse();
            result.ValidationDictionary.ContainsKey(nameof(AddedLegalEntityEvent.LegalEntityId)).Should().BeTrue();
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
            result.IsValid().Should().BeFalse();
            result.ValidationDictionary.ContainsKey(nameof(AddedLegalEntityEvent.OrganisationName)).Should().BeTrue();
        }
    }
}
