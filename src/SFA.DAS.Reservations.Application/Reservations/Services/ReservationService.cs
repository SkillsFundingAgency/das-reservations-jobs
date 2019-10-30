using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Application.Reservations.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _repository;
        private readonly IReservationIndexRepository _indexRepository;
        private readonly IProviderPermissionsRepository _permissionsRepository;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(IReservationRepository repository, IReservationIndexRepository indexRepository,
            IProviderPermissionsRepository permissionsRepository, ILogger<ReservationService> logger)
        {
            _repository = repository;
            _indexRepository = indexRepository;
            _permissionsRepository = permissionsRepository;
            _logger = logger;
        }

        public async Task UpdateReservationStatus(Guid reservationId, ReservationStatus status)
        {
            if (reservationId == null || reservationId.Equals(Guid.Empty))
            {
                throw new ArgumentException("Reservation ID must be set", nameof(reservationId));
            }

            await _repository.SaveStatus(reservationId, status);
        }

        public async Task RefreshReservationIndex()
        {
            try
            {
                var permissions = _permissionsRepository.GetAllWithCreateCohortPermission()?.ToArray();
                var reservationIndexes = new List<ReservationIndex>();

                if (permissions != null)
                {
                    foreach (var permission in permissions)
                    {
                        var matchingReservations = _repository.GetAllNonLevyForAccountLegalEntity(permission.AccountLegalEntityId)?.ToList();
                        if (matchingReservations != null && matchingReservations.Any())
                        {
                            reservationIndexes.AddRange(matchingReservations.Select(c =>
                                MapReservation(c, Convert.ToUInt32(permission.ProviderId))));
                        }
                    }
                }

                await _indexRepository.CreateIndex();

                await _indexRepository.Add(reservationIndexes);
            }
            catch (Exception e)
            {
                _logger.LogError("ReservationService: Unable to create new index", e);
                throw;
            }
        }

        private static ReservationIndex MapReservation(Reservation entity, uint indexedProviderId)
        {
            return new ReservationIndex
            {
                IndexedProviderId = indexedProviderId,
                ReservationId = entity.Id,
                AccountId = entity.AccountId,
                IsLevyAccount = entity.IsLevyAccount,
                CreatedDate = entity.CreatedDate,
                StartDate = entity.StartDate,
                ExpiryDate = entity.ExpiryDate,
                Status = entity.Status,
                CourseName = entity.Course?.Title,
                AccountLegalEntityId = entity.AccountLegalEntityId,
                ProviderId = entity.ProviderId,
                AccountLegalEntityName = entity.AccountLegalEntityName,
                TransferSenderAccountId = entity.TransferSenderAccountId,
                UserId = entity.UserId
            };
        }
    }
}