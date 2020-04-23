using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationRepository
    {
        Task Update(Guid reservationId, ReservationStatus status);
        IEnumerable<Entities.Reservation> GetAllNonLevyForAccountLegalEntity(long accountLegalEntityId);
    }
}
