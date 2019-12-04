using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests
{
    public class TestData
    {
        
        public AccountLegalEntity AccountLegalEntity { get; set; }
        public Course Course { get; set; }
        public Guid ReservationId { get ; set ; }
        public Reservation Reservation { get; set; }
        public ReservationCreatedEvent ReservationCreatedEvent { get; set; }
        public ReservationDeletedEvent ReservationDeletedEvent { get; set; }
        public ProviderPermission ProviderPermission { get; set; }
        public UserDetails UserDetails { get; set; }
    }
}
