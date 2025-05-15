using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Domain.Interfaces;

public interface IReservationService
{
    Task<Reservation> GetReservation(Guid reservationId);
    Task UpdateReservationStatus(Guid reservationId, ReservationStatus status, DateTime? confirmedDate = null, long? cohortId = null, long? draftApprenticeshipId = null);
    Task RefreshReservationIndex();
    Task AddReservationToReservationsIndex(Reservation reservation);
    Task DeleteProviderFromSearchIndex(uint ukPrn, long accountLegalEntityId);
    Task AddProviderToSearchIndex(uint providerId, long accountLegalEntityId);
} 