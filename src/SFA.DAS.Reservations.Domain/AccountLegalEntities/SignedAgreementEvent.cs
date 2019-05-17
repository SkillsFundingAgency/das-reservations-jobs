namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public class SignedAgreementEvent
    {
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
    }
}
