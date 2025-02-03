using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.ProviderPermissions.Service
{
    public class ProviderPermissionService : IProviderPermissionService
    {
        private readonly IProviderPermissionRepository _permissionRepository;
        private readonly ILogger<ProviderPermissionService> _logger;
        private readonly IReservationService _reservationService;

        private const int UniqueConstraintViolation = 2601;
        private const int UniqueKeyViolation = 2627;

        public ProviderPermissionService(
            IProviderPermissionRepository permissionRepository,
            ILogger<ProviderPermissionService> logger, 
            IReservationService reservationService)
        {
            _permissionRepository = permissionRepository;
            _logger = logger;
            _reservationService = reservationService;
        }

        public async Task AddProviderPermission(UpdatedPermissionsEvent updateEvent)
        {
            try
            {
                _logger.LogInformation($"AccountId = {updateEvent.AccountId}, ProviderId = {updateEvent.Ukprn}, CanCreateCohort = {updateEvent.GrantedOperations?.Contains(Operation.CreateCohort)}");
                var permission = Map(updateEvent);

                await _permissionRepository.Add(permission);

                if (!permission.CanCreateCohort)
                {
                    _logger.LogInformation($"Deleting for AccountId = {updateEvent.AccountId}, ProviderId = {updateEvent.Ukprn}");
                    await _reservationService.DeleteProviderFromSearchIndex(
                        Convert.ToUInt32(permission.ProviderId),
                        permission.AccountLegalEntityId);
                }
                else
                {
                    _logger.LogInformation($"Adding for AccountId = {updateEvent.AccountId}, ProviderId = {updateEvent.Ukprn}");
                    await _reservationService.AddProviderToSearchIndex(
                        (uint) permission.ProviderId,
                        permission.AccountLegalEntityId);
                }
            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is SqlException sqlException
                    && (sqlException.Number == UniqueConstraintViolation ||
                        sqlException.Number == UniqueKeyViolation))
                {
                    _logger.LogWarning(
                        $"ProviderPermissionService: Rolling back adding permission for ProviderId:[{updateEvent.Ukprn}], AccountLegalEntityId:[{updateEvent.AccountLegalEntityId}] - item already exists.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error updating index for ProviderId:[{updateEvent.Ukprn}], AccountLegalEntityId:[{updateEvent.AccountLegalEntityId}]", ex);
            }
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
                ProviderId = updateEvent.Ukprn,
                CanCreateCohort = updateEvent.GrantedOperations?.Contains(Operation.CreateCohort) ?? false
            };
        }
    }
}
