namespace SFA.DAS.Reservations.Domain.Configuration
{
    public class ReservationsJobs
    {
        public string ApprenticeshipBaseUrl { get; set; }
        public string ConnectionString { get; set; }
        public string NServiceBusConnectionString { get; set; }
        public string AzureServiceBusConnectionString { get; set; }
    }
}
