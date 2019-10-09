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
            var permission = Map(updateEvent);

            await _repository.Add(permission);
        }

        private static ProviderPermission Map(UpdatedPermissionsEvent updateEvent)
        { 
            return new ProviderPermission
            {
                AccountId = updateEvent.AccountId,
                AccountLegalEntityId = updateEvent.AccountLegalEntityId,
                UkPrn = updateEvent.Ukprn,
                CanCreateCohort = updateEvent.GrantedOperations.Contains(Operation.CreateCohort)
            };
        }
    }
}
