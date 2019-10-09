using System;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class ProviderPermission
    {
        public long UkPrn { get; set; }
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public bool CanCreateCohort { get; set; }
    }
}
