using System;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Functions.ReservationIndex.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddElasticSearch(this ServiceCollection collection, ReservationsJobs configuration)
        {
            var connectionPool = new  SingleNodeConnectionPool(new Uri(configuration.ElasticSearchUrl));

            var settings = new ConnectionSettings(connectionPool);

            collection.AddTransient<IElasticClient>(sp => new ElasticClient(settings));
        }

    }
}
