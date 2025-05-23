﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;


namespace SFA.DAS.Reservations.Data.Repository
{
    public class ReservationRepository(IReservationsDataContext dataContext) : IReservationRepository
    {
        public async Task Update(
            Guid reservationId,
            ReservationStatus status,
            DateTime? confirmedDate = null,
            long? cohortId = null,
            long? draftApprenticeshipId = null)
        {

                var reservation = await dataContext.Reservations.FindAsync(reservationId);

                if (reservation == null)
                {
                    throw new InvalidOperationException($"Reservation not found in database with ReservationId: {reservationId}");
                }

                if (!reservation.IsLevyAccount)
                {
                    if (status == ReservationStatus.Pending
                        && reservation.Status != (short)ReservationStatus.Confirmed
                        && !reservation.ConfirmedDate.HasValue
                        && !reservation.CohortId.HasValue
                        && !reservation.DraftApprenticeshipId.HasValue
                    )
                    {
                        throw new DbUpdateException($"Unable to change reservation {reservationId} to pending as it has not been confirmed", (Exception)null);
                    }
                }

                // do not update status of 'change' reservation unless deleting.
                if (reservation.Status != (short)ReservationStatus.Change || status == ReservationStatus.Deleted)
                {
                    reservation.Status = (short)status;
                }

                switch (status)
                {
                    case ReservationStatus.Confirmed:
                        reservation.ConfirmedDate = confirmedDate;
                        reservation.CohortId = cohortId;
                        reservation.DraftApprenticeshipId = draftApprenticeshipId;
                        break;
                    case ReservationStatus.Pending:
                    case ReservationStatus.Deleted:
                        reservation.ConfirmedDate = null;
                        reservation.CohortId = null;
                        reservation.DraftApprenticeshipId = null;
                        break;
                }

                dataContext.SaveChanges();
              
        }

        public IEnumerable<Reservation> GetAllNonLevyForAccountLegalEntity(long accountLegalEntityId)
        {
            return dataContext.Reservations
                .Where(c => c.AccountLegalEntityId.Equals(accountLegalEntityId)
                            && c.Status != (byte)ReservationStatus.Deleted
                            && c.Status != (byte)ReservationStatus.Change
                            && !c.IsLevyAccount);
        }

        public async Task<Reservation> GetReservationById(Guid reservationId)
        {
            var reservation = await dataContext.Reservations
                .FirstOrDefaultAsync(c => c.Id.Equals(reservationId))
                .ConfigureAwait(false);

            return reservation;
        }

    }
}
