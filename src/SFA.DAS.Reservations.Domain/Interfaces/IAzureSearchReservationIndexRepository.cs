using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Domain.Interfaces;

public interface IAzureSearchReservationIndexRepository
{
    Task<string> CreateIndex();
    Task Add(IEnumerable<IndexedReservation> reservations, string? indexName = null);
    Task DeleteIndices(uint daysOld);
    Task DeleteReservationsFromIndex(uint ukPrn, long accountLegalEntityId);
    Task SaveReservationStatus(Guid id, ReservationStatus status);
    Task UpdateAlias(string indexName);
} 