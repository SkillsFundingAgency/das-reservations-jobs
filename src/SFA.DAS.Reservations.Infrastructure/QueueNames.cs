namespace SFA.DAS.Reservations.Infrastructure
{
    public static class QueueNames
    {
        public const string GetCourses = "sfa-das-rsrv-get-course";
        public const string StoreCourse = "sfa-das-rsrv-store-course";
        public const string ConfirmReservation = "sfa-das-rsrv-confirm-reservation";
        public const string AccountsEndpoint = "SFA.DAS.Reservations.Jobs";
        public const string LegalEntityAdded = "sfa-das-rsrv-legal-entity-added";
        public const string RemovedLegalEntity = "sfa-das-rsrv-remove-legal-entity";
        public const string SignedAgreement = "sfa-das-rsrv-signed-agreement";
    }
}
