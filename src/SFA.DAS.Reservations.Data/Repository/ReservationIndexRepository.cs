using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ReservationIndexRepository : IReservationIndexRepository
    {
        public Task Add(IEnumerable<ReservationIndex> reservations)
        {
            throw new System.NotImplementedException();
        }

        public Task Add(ReservationIndex reservations)
        {
            throw new System.NotImplementedException();
        }

        public Task Clear()
        {
            throw new System.NotImplementedException();
        }
    }
}
