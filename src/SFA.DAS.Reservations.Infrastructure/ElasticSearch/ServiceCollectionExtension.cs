using System;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Infrastructure.ElasticSearch
{
    public static class ServiceCollectionExtension
    {
        public static void AddElasticSearch(
            this ServiceCollection collection,
            ReservationsJobs configuration, 
            string environmentName)
        {
            var connectionPool = new SingleNodeConnectionPool(new Uri(configuration.ElasticSearchServerUrl));

            var settings = new ConnectionSettings(connectionPool);
            settings.DefaultMappingFor<IndexedReservation>(descriptor => descriptor.IndexName($"{environmentName}-reservations"));

            if (!string.IsNullOrEmpty(configuration.ElasticSearchUsername) &&
                !string.IsNullOrEmpty(configuration.ElasticSearchPassword))
            {
                settings.BasicAuthentication(configuration.ElasticSearchUsername, configuration.ElasticSearchPassword);
            }


            collection.AddTransient<IElasticClient>(sp => new ElasticClient(settings));
        }
    }
}
