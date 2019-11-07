using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationIndexRepository
    {
        Task CreateIndex();

        Task Add(IEnumerable<ReservationIndex> reservations);
        Task Add(ReservationIndex reservations);
        Task DeleteIndices(uint daysOld);
    }
}
