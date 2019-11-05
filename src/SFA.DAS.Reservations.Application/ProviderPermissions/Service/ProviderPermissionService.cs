﻿using System;
using System.Threading.Tasks;
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
        private readonly IProviderPermissionRepository _repository;
        private readonly ILogger<ProviderPermissionService> _logger;
        private readonly IReservationService _reservationService;

        public ProviderPermissionService(IProviderPermissionRepository repository,
            ILogger<ProviderPermissionService> logger, IReservationService reservationService)
        {
            _repository = repository;
            _logger = logger;
            _reservationService = reservationService;
        }

        public async Task AddProviderPermission(UpdatedPermissionsEvent updateEvent)
        {
            try
            {
                ValidateEvent(updateEvent);
            }
            catch (ArgumentException e)
            {
                _logger.LogWarning($"Cannot process provider permission message due to missing argument {e.ParamName}. " +
                                   $"Account ID: {updateEvent?.AccountId}, Ukprn: {updateEvent?.Ukprn}, Account legal entity ID:{updateEvent?.AccountLegalEntityId}");
                return;
            }

            var permission = Map(updateEvent);

            if (!permission.CanCreateCohort)
            {
                await _reservationService.DeleteProviderFromSearchIndex(
                    Convert.ToUInt32(permission.ProviderId),
                    permission.AccountLegalEntityId);
            }

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
                ProviderId = updateEvent.Ukprn,
                CanCreateCohort = updateEvent.GrantedOperations?.Contains(Operation.CreateCohort) ?? false
            };
        }
    }
}
