namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class SignedAgreementEvent
    {
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
    }
}
