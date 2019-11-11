using System;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Infrastructure.ElasticSearch
{
    public static class ServiceCollectionExtension
    {
        public static void AddElasticSearch(this ServiceCollection collection, ReservationsJobs configuration)
        {
            var connectionPool = new SingleNodeConnectionPool(new Uri(configuration.ElasticSearchServerUrl));

            var settings = new ConnectionSettings(connectionPool);

            if (!string.IsNullOrEmpty(configuration.ElasticSearchUsername) &&
                !string.IsNullOrEmpty(configuration.ElasticSearchPassword))
            {
                settings.BasicAuthentication(configuration.ElasticSearchUsername, configuration.ElasticSearchPassword);
            }


            collection.AddTransient<IElasticClient>(sp => new ElasticClient(settings));
        }
    }
}
