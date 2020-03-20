namespace SFA.DAS.Reservations.Domain.Infrastructure
{
    public interface IElasticSearchQueries
    {
        string ReservationIndexLookupName { get; }

        string LastIndexSearchQuery { get; }
        
    }
}