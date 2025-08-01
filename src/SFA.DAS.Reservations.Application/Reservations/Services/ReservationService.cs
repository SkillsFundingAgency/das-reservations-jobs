using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Services;

public class ReservationService(
    IReservationRepository reservationsRepository,
    IAzureSearchReservationIndexRepository azureSearchIndexRepository,
    IProviderPermissionRepository permissionsRepository,
    ILogger<ReservationService> logger)
    : IReservationService
{
    public async Task<Reservation> GetReservation(Guid reservationId)
    {
        if (reservationId == null || reservationId.Equals(Guid.Empty))
            throw new ArgumentException("Reservation ID must be set", nameof(reservationId));

        var reservation = await reservationsRepository.GetReservationById(reservationId);

        if (reservation is null)
        {
            logger.LogWarning("Reservation {ReservationId} was not found in the database", reservationId);
            return null;
        }

        return new Reservation(reservation);
    }

    public async Task UpdateReservationStatus(Guid reservationId, ReservationStatus status,
        DateTime? confirmedDate = null, long? cohortId = null, long? draftApprenticeshipId = null)
    {
        if (reservationId == null || reservationId.Equals(Guid.Empty))
        {
            throw new ArgumentException("Reservation ID must be set", nameof(reservationId));
        }

        try
        {
            await reservationsRepository.Update(
                reservationId,
                status,
                confirmedDate,
                cohortId,
                draftApprenticeshipId);
            await azureSearchIndexRepository.SaveReservationStatus(reservationId, status);
        }
        catch (InvalidOperationException e)
        {
            logger.LogWarning(
                "Reservation {ReservationId} was not found in the database and not updated to {Status}",
                reservationId, status);
        }
    }

    public async Task RefreshReservationIndex()
    {
        try
        {
            var indexName = await azureSearchIndexRepository.CreateIndex();

            var permissions = permissionsRepository.GetAllWithCreateCohortPermission().ToList();

            if (permissions != null)
            {
                foreach (var permission in permissions)
                {
                    logger.LogInformation(
                        "Adding reservations to index for ({Ukprn}) ({AccountId}) ({AccountLegalEntityId})",
                        permission.ProviderId,
                        permission.AccountId,
                        permission.AccountLegalEntityId);

                    var matchingReservations = reservationsRepository
                        .GetAllNonLevyForAccountLegalEntity(permission.AccountLegalEntityId)?.ToList();

                    if (matchingReservations == null || matchingReservations.Count == 0)
                    {
                        logger.LogInformation(
                            "No reservations to add for ({Ukprn}) ({AccountId}) ({AccountLegalEntityId})",
                            permission.ProviderId,
                            permission.AccountId,
                            permission.AccountLegalEntityId);
                        continue;
                    }

                    var indexedReservations = matchingReservations.ConvertAll(c =>
                        MapReservation(c, Convert.ToUInt32(permission.ProviderId)));

                    await azureSearchIndexRepository.Add(indexedReservations, indexName);
                }
            }

            await azureSearchIndexRepository.UpdateAlias(indexName);
            await azureSearchIndexRepository.DeleteIndices(5);

            logger.LogInformation("Successfully refreshed reservations indices");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error refreshing reservations indices");
            throw;
        }
    }

    public async Task AddReservationToReservationsIndex(Reservation reservation)
    {
        logger.LogInformation("Adding Reservation Id [{ReservationId}] to index.", reservation.Id);

        var permissions = permissionsRepository.GetAllForAccountLegalEntity(reservation.AccountLegalEntityId)
            .ToArray();

        logger.LogInformation("[{NumberOfPermissions}] providers found for Reservation Id [{ReservationId}].",
            permissions.Length, reservation.Id);

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

        await azureSearchIndexRepository.Add(indexedReservations);

        logger.LogInformation(
            $"[{indexedReservations.Count}] new documents have been created for Reservation Id [{reservation.Id}].");
    }

    public async Task DeleteProviderFromSearchIndex(uint ukPrn, long accountLegalEntityId)
    {
        logger.LogInformation(
            "Deleting reservations for ProviderId [{UkPrn}], AccountLegalEntityId [{AccountLegalEntityId}] from index.",
            ukPrn, accountLegalEntityId);

        await azureSearchIndexRepository.DeleteReservationsFromIndex(ukPrn, accountLegalEntityId);
    }

    public async Task AddProviderToSearchIndex(uint providerId, long accountLegalEntityId)
    {
        logger.LogInformation(
            "Adding reservations for ProviderId [{ProviderId}], AccountLegalEntityId [{AccountLegalEntityId}] to index.",
            providerId, accountLegalEntityId);

        var indexedReservations = new List<IndexedReservation>();

        var matchingReservations = reservationsRepository.GetAllNonLevyForAccountLegalEntity(accountLegalEntityId)
            ?.ToList();

        logger.LogInformation(
            "[{MatchingReservationsCount}] providers found for ProviderId [{ProviderId}], AccountLegalEntityId [{AccountLegalEntityId}].",
            matchingReservations.Count, providerId, accountLegalEntityId);

        if (matchingReservations != null && matchingReservations.Any())
        {
            indexedReservations.AddRange(matchingReservations.Select(c =>
                MapReservation(c, providerId)));

            await azureSearchIndexRepository.Add(indexedReservations);
        }

        logger.LogInformation(
            "[{IndexedReservationsCount}] new documents have been created for ProviderId [{ProviderId}], AccountLegalEntityId [{AccountLegalEntityId}].",
            indexedReservations.Count, providerId, accountLegalEntityId);
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