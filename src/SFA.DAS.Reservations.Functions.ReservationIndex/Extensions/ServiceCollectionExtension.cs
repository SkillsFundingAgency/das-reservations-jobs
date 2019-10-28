using System;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace SFA.DAS.Reservations.Functions.ReservationIndex.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddElasticSearch(this ServiceCollection collection)
        {
            var connectionPool = new  SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var settings = new ConnectionSettings(connectionPool);

            collection.AddTransient<IElasticClient>(sp => new ElasticClient(settings));
        }

    }
}
