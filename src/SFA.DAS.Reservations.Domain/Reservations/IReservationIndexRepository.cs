using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationIndexRepository
    {
        Task CreateIndex();

        Task Add(IEnumerable<IndexedReservation> reservations);
        Task Add(IndexedReservation reservation);
        Task DeleteIndices(uint daysOld);
        Task DeleteReservationsFromIndex(uint ukPrn, long accountLegalEntityId);
        Task SaveReservationStatus(Guid id, ReservationStatus status);
    }
}
