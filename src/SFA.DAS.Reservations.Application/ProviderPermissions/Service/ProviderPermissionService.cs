using System;
using System.Threading.Tasks;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Application.ProviderPermissions.Service
{
    public class ProviderPermissionService : IProviderPermissionService
    {
        private readonly IProviderPermissionRepository _repository;

        public ProviderPermissionService(IProviderPermissionRepository repository)
        {
            _repository = repository;
        }

        public async Task AddProviderPermission(UpdatedPermissionsEvent updateEvent)
        {
            ValidateEvent(updateEvent);

            var permission = Map(updateEvent);

            await _repository.Add(permission);
        }

        private void ValidateEvent(UpdatedPermissionsEvent updateEvent)
        {
            if (updateEvent == null)
            {
                throw new ArgumentException(
                    "Event cannot be null",
                    nameof(UpdatedPermissionsEvent));
            }

            if (updateEvent.AccountId.Equals(default(long)))
            {
                throw new ArgumentException(
                    "Account ID must be set in event", 
                    nameof(updateEvent.AccountId));
            }

            if (updateEvent.AccountLegalEntityId.Equals(default(long)))
            {
                throw new ArgumentException(
                    "Account legal entity ID must be set in event", 
                    nameof(updateEvent.AccountLegalEntityId));
            }

            if (updateEvent.Ukprn.Equals(default(long)))
            {
                throw new ArgumentException(
                    "UKPRN must be set in event", 
                    nameof(updateEvent.Ukprn));
            }
        }

        private static ProviderPermission Map(UpdatedPermissionsEvent updateEvent)
        {
            return new ProviderPermission
            {
                AccountId = updateEvent.AccountId,
                AccountLegalEntityId = updateEvent.AccountLegalEntityId,
                UkPrn = updateEvent.Ukprn,
                CanCreateCohort = updateEvent.GrantedOperations?.Contains(Operation.CreateCohort) ?? false
            };
        }
    }
}
