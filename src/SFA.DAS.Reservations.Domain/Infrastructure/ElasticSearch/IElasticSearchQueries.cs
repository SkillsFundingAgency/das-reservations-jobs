namespace SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch
{
    public interface IElasticSearchQueries
    {
        string ReservationIndexLookupName { get; }

        string LastIndexSearchQuery { get; }
        
    }
}