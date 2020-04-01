using System;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Functions.LegalEntities.AcceptanceTests
{
    public class TestData
    {
        
        public AccountLegalEntity AccountLegalEntity { get; set; }
        public Course Course { get; set; }
        public Guid ReservationId { get ; set ; }

        public Exception Exception { get; set; }
        public Account NonLevyAccount { get ; set ; }
        public string NewAccountName { get ; set ; }
    }
}
