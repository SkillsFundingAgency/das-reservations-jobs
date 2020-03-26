namespace SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch
{
    public interface IElasticSearchQueries
    {
        string ReservationIndexLookupName { get; }

        string LastIndexSearchQuery { get; }
        string ReservationIndexMapping { get; }
        string UpdateReservationStatus { get;  }
        string DeleteReservationsByQuery { get; }
    }
}