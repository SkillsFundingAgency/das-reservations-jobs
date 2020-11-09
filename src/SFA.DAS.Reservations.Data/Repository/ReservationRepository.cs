using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
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

        public async Task Update(
            Guid reservationId, 
            ReservationStatus status, 
            DateTime? confirmedDate = null, 
            long? cohortId = null, 
            long? draftApprenticeshipId = null)
        {
            using (var transaction = _dataContext.Database.BeginTransaction())
            {
                var reservation = await _dataContext.Reservations.FindAsync(reservationId);

                if (reservation == null)
                {
                    throw new InvalidOperationException($"Reservation not found in database with ReservationId: {reservationId}");
                }

                if (!reservation.IsLevyAccount)
                {
                    if (status == ReservationStatus.Pending 
                        && reservation.Status != (short) ReservationStatus.Confirmed
                        && !reservation.ConfirmedDate.HasValue
                        && !reservation.CohortId.HasValue
                        && !reservation.DraftApprenticeshipId.HasValue
                    )
                    {
                        throw new DbUpdateException($"Unable to change reservation {reservationId} to pending as it has not been confirmed", (Exception) null);
                    }    
                }
                

                reservation.Status = (short)status;

                switch (status)
                {
                    case ReservationStatus.Confirmed:
                        reservation.ConfirmedDate = confirmedDate;
                        reservation.CohortId = cohortId;
                        reservation.DraftApprenticeshipId = draftApprenticeshipId;
                        break;
                    case ReservationStatus.Pending:
                        reservation.ConfirmedDate = null;
                        reservation.CohortId = null;
                        reservation.DraftApprenticeshipId = null;
                        break;
                }

                _dataContext.SaveChanges();
                transaction.Commit();
            }
        }

        public IEnumerable<Reservation> GetAllNonLevyForAccountLegalEntity(long accountLegalEntityId)
        {
            return _dataContext.Reservations
                .Where(c=>c.AccountLegalEntityId.Equals(accountLegalEntityId) 
                          && c.Status != (byte)ReservationStatus.Deleted
                          && c.Status != (byte)ReservationStatus.Change
                          && !c.IsLevyAccount).ToArray();
        }
    }
}
