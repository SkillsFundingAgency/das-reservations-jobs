namespace SFA.DAS.Reservations.Domain.Configuration
{
    public class ReservationsJobs
    {
        public string ReservationsBaseUrl { get; set; }
        public string ConnectionString { get; set; }
        public string NServiceBusConnectionString { get; set; }
        public string NServiceBusLicense { get; set; }
        public string AzureWebJobsStorage { get; set; }
        public string ElasticSearchUsername { get; set; }
        public string ElasticSearchPassword { get; set; }
        public string ElasticSearchServerUrl { get; set; }
        public string ReservationCreatedEmailTemplateId { get; set; }
        public virtual string ReservationsApimUrl { get; set; }
        public virtual string ReservationsApimSubscriptionKey { get; set; }
    }
}
