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
        private readonly IReservationRepository _reservationsRepository;
        private readonly IReservationIndexRepository _indexRepository;
        private readonly IProviderPermissionRepository _permissionsRepository;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(
            IReservationRepository reservationsRepository, 
            IReservationIndexRepository indexRepository,
            IProviderPermissionRepository permissionsRepository, 
            ILogger<ReservationService> logger)
        {
            _reservationsRepository = reservationsRepository;
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

            await _reservationsRepository.SaveStatus(reservationId, status);
        }

        public async Task RefreshReservationIndex()
        {
            try
            {
                var permissions = _permissionsRepository.GetAllWithCreateCohortPermission();
                var indexedReservations = new List<IndexedReservation>();

                if (permissions != null)
                {
                    foreach (var permission in permissions)
                    {
                        var matchingReservations = _reservationsRepository.GetAllNonLevyForAccountLegalEntity(permission.AccountLegalEntityId)?.ToList();
                        if (matchingReservations != null && matchingReservations.Any())
                        {
                            indexedReservations.AddRange(matchingReservations.Select(c =>
                                MapReservation(c, Convert.ToUInt32(permission.ProviderId))));
                        }
                    }
                }

                if (!indexedReservations.Any())
                {
                    indexedReservations.Add(new IndexedReservation());
                }

                await _indexRepository.CreateIndex();

                await _indexRepository.Add(indexedReservations);
            }
            catch (Exception e)
            {
                _logger.LogError($"ReservationService: Unable to create new index: {e.Message}", e);
                throw;
            }
        }

        public async Task AddReservationToReservationsIndex(Domain.Reservations.Reservation reservation)
        {
            var permissions = _permissionsRepository.GetAllForAccountLegalEntity(reservation.AccountLegalEntityId);
            var indexedReservations = new List<IndexedReservation>();

            foreach (var providerPermission in permissions)
            {
                IndexedReservation indexedReservation = reservation;
                indexedReservation.IndexedProviderId = (uint) providerPermission.ProviderId;
                indexedReservations.Add(indexedReservation);
            }

            await _indexRepository.Add(indexedReservations);
        }
        public async Task DeleteProviderFromSearchIndex(uint ukPrn, long accountLegalEntityId)
        {
            await _indexRepository.DeleteReservationsFromIndex(ukPrn, accountLegalEntityId);
        }

        private static IndexedReservation MapReservation(Reservation entity, uint indexedProviderId)
        {
            return new IndexedReservation
            {
                IndexedProviderId = indexedProviderId,
                ReservationId = entity.Id,
                AccountId = entity.AccountId,
                IsLevyAccount = entity.IsLevyAccount,
                CreatedDate = entity.CreatedDate,
                StartDate = entity.StartDate,
                ExpiryDate = entity.ExpiryDate,
                Status = entity.Status,
                CourseId = entity.CourseId,
                CourseTitle = entity.Course?.Title,
                CourseLevel = entity.Course?.Level,
                AccountLegalEntityId = entity.AccountLegalEntityId,
                ProviderId = entity.ProviderId,
                AccountLegalEntityName = entity.AccountLegalEntityName,
                TransferSenderAccountId = entity.TransferSenderAccountId,
                UserId = entity.UserId
            };
        }
    }
}