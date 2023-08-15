using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;

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

        public async Task<Reservation> GetReservation(Guid reservationId)
        {
            if (reservationId == null || reservationId.Equals(Guid.Empty))
                throw new ArgumentException("Reservation ID must be set", nameof(reservationId));

            var reservation = await _reservationsRepository.GetReservationById(reservationId);

            if (reservation is null)
            {
                _logger.LogWarning($"Reservation {reservationId} was not found in the database");
                return null;
            }

            return new Reservation(reservation);
        }

        public async Task UpdateReservationStatus(Guid reservationId, ReservationStatus status, DateTime? confirmedDate = null, long? cohortId = null, long? draftApprenticeshipId = null)
        {
            if (reservationId == null || reservationId.Equals(Guid.Empty))
            {
                throw new ArgumentException("Reservation ID must be set", nameof(reservationId));
            }

            try
            {
                await _reservationsRepository.Update(
                    reservationId,
                    status,
                    confirmedDate,
                    cohortId,
                    draftApprenticeshipId);
                await _indexRepository.SaveReservationStatus(reservationId, status);

            }
            catch (InvalidOperationException e)
            {
                _logger.LogWarning($"Reservation {reservationId} was not found in the database and not updated to {status}");
            }
        }

        public async Task RefreshReservationIndex()
        {
            try
            {
                var permissions = _permissionsRepository.GetAllWithCreateCohortPermission();
                var indexedReservations = new List<IndexedReservation>();

                if (permissions != null)
                {
                    var indexedReservationsTasks = new List<Task<List<IndexedReservation>>>();

                    foreach (var permission in permissions)
                    {
                        indexedReservationsTasks.Add(ProcessPermissionAsync(permission));
                    }

                    var indexedReservationsLists = await Task.WhenAll(indexedReservationsTasks);

                    indexedReservations = indexedReservationsLists.SelectMany(list => list).ToList();                                       
                }

                if (!indexedReservations.Any())
                {
                    indexedReservations.Add(new IndexedReservation());
                }

                await _indexRepository.CreateIndex();

                await _indexRepository.Add(indexedReservations);

                await _indexRepository.DeleteIndices(5);
            }
            catch (Exception e)
            {
                _logger.LogError($"ReservationService: Unable to create new index: {e.Message}", e);
                throw;
            }
        }
        
        private async Task<List<IndexedReservation>> ProcessPermissionAsync(Domain.Entities.ProviderPermission permission)
        {
            var indexedReservations = new List<IndexedReservation>();
            var matchingReservations = await _reservationsRepository.GetAllNonLevyForAccountLegalEntity(permission.AccountLegalEntityId);
    
            if (matchingReservations != null && matchingReservations.Any())
            {
                indexedReservations.AddRange(matchingReservations.Select(c =>
                    MapReservation(c, Convert.ToUInt32(permission.ProviderId))));
            }

            return indexedReservations;
        }

        public async Task AddReservationToReservationsIndex(Domain.Reservations.Reservation reservation)
        {
            _logger.LogInformation($"Adding Reservation Id [{reservation.Id}] to index.");

            var permissions = _permissionsRepository.GetAllForAccountLegalEntity(reservation.AccountLegalEntityId).ToArray();

            _logger.LogInformation($"[{permissions.Length}] providers found for Reservation Id [{reservation.Id}].");

            if (!permissions.Any())
            {
                return;
            }

            var indexedReservations = new List<IndexedReservation>();
            foreach (var providerPermission in permissions)
            {
                IndexedReservation indexedReservation = reservation;
                indexedReservation.IndexedProviderId = (uint)providerPermission.ProviderId;
                indexedReservations.Add(indexedReservation);
            }

            await _indexRepository.Add(indexedReservations);

            _logger.LogInformation($"[{indexedReservations.Count}] new documents have been created for Reservation Id [{reservation.Id}].");
        }

        public async Task DeleteProviderFromSearchIndex(uint ukPrn, long accountLegalEntityId)
        {
            _logger.LogInformation($"Deleting reservations for ProviderId [{ukPrn}], AccountLegalEntityId [{accountLegalEntityId}] from index.");

            await _indexRepository.DeleteReservationsFromIndex(ukPrn, accountLegalEntityId);
        }

        public async Task AddProviderToSearchIndex(uint providerId, long accountLegalEntityId)
        {
            _logger.LogInformation($"Adding reservations for ProviderId [{providerId}], AccountLegalEntityId [{accountLegalEntityId}] to index.");

            var indexedReservations = new List<IndexedReservation>();

            var matchingReservations = await _reservationsRepository.GetAllNonLevyForAccountLegalEntity(accountLegalEntityId);

            _logger.LogInformation($"[{matchingReservations.Count}] providers found for ProviderId [{providerId}], AccountLegalEntityId [{accountLegalEntityId}].");

            if (matchingReservations != null && matchingReservations.Any())
            {
                indexedReservations.AddRange(matchingReservations.Select(c =>
                    MapReservation(c, providerId)));

                await _indexRepository.Add(indexedReservations);
            }

            _logger.LogInformation($"[{indexedReservations.Count}] new documents have been created for ProviderId [{providerId}], AccountLegalEntityId [{accountLegalEntityId}].");
        }

        private static IndexedReservation MapReservation(Domain.Entities.Reservation entity, uint indexedProviderId)
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