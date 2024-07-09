using System;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Data.ElasticSearch;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;

namespace SFA.DAS.Reservations.Infrastructure.ElasticSearch
{
    public static class ServiceCollectionExtension
    {
        public static void AddElasticSearch(
            this ServiceCollection collection,
            ReservationsJobs configuration)
        {
            var connectionPool = new SingleNodeConnectionPool(new Uri(configuration.ElasticSearchServerUrl));

            var settings = new ConnectionConfiguration(connectionPool);

            if (!string.IsNullOrEmpty(configuration.ElasticSearchUsername) &&
                !string.IsNullOrEmpty(configuration.ElasticSearchPassword))
            {
                settings.BasicAuthentication(configuration.ElasticSearchUsername, configuration.ElasticSearchPassword);
            }
            
            collection.AddTransient<IElasticLowLevelClient>(sp => new ElasticLowLevelClient(settings));
            collection.AddSingleton<IElasticSearchQueries, ElasticSearchQueries>();
            collection.AddTransient<IElasticLowLevelClientWrapper,ElasticLowLevelClientWrapper>();
        }
    }
}
