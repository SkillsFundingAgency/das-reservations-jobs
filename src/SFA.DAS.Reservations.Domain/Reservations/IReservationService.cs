using System;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationService
    {
        Task UpdateReservationStatus(Guid reservationId, ReservationStatus status, DateTime? confirmedDate = null, long? cohortId = null, long? draftApprenticeshipId = null);
        Task RefreshReservationIndex();
        Task AddReservationToReservationsIndex(Reservation reservation);
        Task DeleteProviderFromSearchIndex(uint ukPrn, long accountLegalEntityId);
        Task AddProviderToSearchIndex(uint providerId, long accountLegalEntityId);
        Task UpdateReservationStatus(Guid reservationId, DateTime? confirmedDate = null, long? cohortId = null, long? draftApprenticeshipId = null);
    }
}
