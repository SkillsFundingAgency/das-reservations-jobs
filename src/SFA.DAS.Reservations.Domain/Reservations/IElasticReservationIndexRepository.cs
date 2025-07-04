﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IElasticReservationIndexRepository
    {
        Task CreateIndex();

        Task Add(IEnumerable<IndexedReservation> reservations);
        Task DeleteIndices(uint daysOld);
        Task DeleteReservationsFromIndex(uint ukPrn, long accountLegalEntityId);
        Task SaveReservationStatus(Guid id, ReservationStatus status);
    }
}
