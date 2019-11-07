namespace SFA.DAS.Reservations.Infrastructure
{
    public static class QueueNames
    {
        public const string GetCourses = "sfa-das-rsrv-get-course";
        public const string StoreCourse = "sfa-das-rsrv-store-course";
        public const string RefreshReservationIndex = "sfa-das-rsrv-refresh-index";
        public const string ConfirmReservation = "SFA.DAS.Reservations.ConfirmReservation";
        public const string LegalEntityAdded = "SFA.DAS.Reservations.Jobs.LegalEntityAdded";
        public const string RemovedLegalEntity = "SFA.DAS.Reservations.Jobs.LegalEntityRemoved";
        public const string SignedAgreement = "SFA.DAS.Reservations.Jobs.SignedAgreement";
        public const string DraftApprenticeshipDeleted = "SFA.DAS.Reservations.Jobs.ApprenticeshipDeleted";
        public const string LevyAddedToAccount = "SFA.DAS.Reservations.Jobs.LevyAddedToAccount";
		public const string UpdatedProviderPermissions = "SFA.DAS.Reservations.Jobs.PrlUpdated";
        public const string ReservationCreated = "SFA.DAS.Reservations.Jobs.ReservationCreated";
        public const string ReservationDeleted = "SFA.DAS.Reservations.Jobs.ReservationDeleted";
    }
}
