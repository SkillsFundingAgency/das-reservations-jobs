using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationRepository
    {
        Task Update(Guid reservationId, ReservationStatus status, DateTime? confirmedDate = null, long? cohortId = null, long? draftApprenticeshipId = null);
        Task<List<Entities.Reservation>> GetAllNonLevyForAccountLegalEntity(long accountLegalEntityId);
        Task<Entities.Reservation> GetReservationById(Guid reservationId);
    }
}
