namespace SFA.DAS.Reservations.Infrastructure
{
    public static class QueueNames
    {
        public const string GetCourses = "sfa-das-rsrv-get-course";
        public const string StoreCourse = "sfa-das-rsrv-store-course";
        public const string ConfirmReservation = "sfa-das-rsrv-confirm-reservation";
        public const string AccountsEndpoint = "sfa.das.employeraccounts.messagehandlers";
    }
}
