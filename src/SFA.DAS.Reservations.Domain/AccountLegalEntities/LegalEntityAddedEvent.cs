namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class AccountLegalEntityAddedEvent
    {
        public long AccountId { get; set; }
        public string OrganisationName { get; set; }
        public long LegalEntityId { get; set; }
        public long AccountLegalEntityId { get; set; }
    }
}