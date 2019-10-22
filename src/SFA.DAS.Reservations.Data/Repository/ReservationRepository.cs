using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            using (var transaction = _dataContext.Database.BeginTransaction())
            {
                var reservation = await _dataContext.Reservations.FindAsync(reservationId);

                if (reservation == null)
                {
                    throw new InvalidOperationException($"Reservation not found in database with Id: {reservationId}");
                }

                reservation.Status = (short)status;

                _dataContext.SaveChanges();
                transaction.Commit();
            }

            
        }

        public Task<IEnumerable<Domain.Entities.Reservation>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
