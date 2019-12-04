using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Validators
{
    public class AddAccountLegalEntityValidator : IValidator<AddedLegalEntityEvent>
    {
        public Task<ValidationResult> ValidateAsync(AddedLegalEntityEvent @event)
        {
            var validationResult = new ValidationResult();

            if (@event.AccountId == default)
            {
                validationResult.AddError(nameof(@event.AccountId));
            }

            if (@event.AccountLegalEntityId == default)
            {
                validationResult.AddError(nameof(@event.AccountLegalEntityId));
            }

            if (@event.LegalEntityId == default)
            {
                validationResult.AddError(nameof(@event.LegalEntityId));
            }

            if(string.IsNullOrWhiteSpace(@event.OrganisationName))
            {
                validationResult.AddError(nameof(@event.OrganisationName));
            }

            return Task.FromResult(validationResult);
        }
    }
}
