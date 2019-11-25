using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;


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
                    throw new InvalidOperationException($"Reservation not found in database with ReservationId: {reservationId}");
                }

                reservation.Status = (short)status;

                _dataContext.SaveChanges();
                transaction.Commit();
            }
        }

        public IEnumerable<Reservation> GetAllNonLevyForAccountLegalEntity(long accountLegalEntityId)
        {
            return _dataContext.Reservations
                .Where(c=>c.AccountLegalEntityId.Equals(accountLegalEntityId) 
                          && c.Status != (byte)ReservationStatus.Deleted
                          && !c.IsLevyAccount).ToArray();
        }
    }
}
