using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests
{
    public class TestData
    {
        
        public AccountLegalEntity AccountLegalEntity { get; set; }
        public Course Course { get; set; }
        public Guid ReservationId { get ; set ; }
    }
}
