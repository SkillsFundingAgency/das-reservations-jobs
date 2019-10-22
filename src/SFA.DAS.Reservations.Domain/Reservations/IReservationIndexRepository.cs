using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationIndexRepository
    {
        Task Add(IEnumerable<ReservationIndex> reservations);
        Task Add(ReservationIndex reservations);
        Task Clear();
    }
}
