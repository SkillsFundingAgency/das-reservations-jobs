using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Reservations;


namespace SFA.DAS.Reservations.Data.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly IReservationsDataContext _dataContext;


        public ReservationRepository(IReservationsDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task SaveStatus(Guid reservationId, ReservationStatus status)
        {
            var reservation = await _dataContext.Reservations.SingleOrDefaultAsync(r => r.Id.Equals(reservationId));

            if (reservation == null)
            {
                throw new InvalidOperationException($"Reservation not found in database with Id: {reservationId}");
            }

            reservation.Status = (short)status;

            _dataContext.SaveChanges();
        }
    }
}
