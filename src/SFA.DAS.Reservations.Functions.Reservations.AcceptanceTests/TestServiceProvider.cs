using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests
{
    public class TestServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public TestServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            var configuration = GenerateConfiguration();

            var serviceProviderBuilder = new ServiceProviderBuilder(new LoggerFactory(), configuration);
           
            _serviceProvider = serviceProviderBuilder.Build();
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
       
        private static IConfigurationRoot GenerateConfiguration()
        {
            var configSource = new MemoryConfigurationSource
            {
                InitialData = new[]
                {
                    new KeyValuePair<string, string>("ConfigurationStorageConnectionString", "UseDevelopmentStorage=true;"),
                    new KeyValuePair<string, string>("ConfigNames", "SFA.DAS.Reservations.Api"),
                    new KeyValuePair<string, string>("Environment", "DEV"),
                    new KeyValuePair<string, string>("Version", "1.0"),
                    new KeyValuePair<string, string>("Reservations:ElasticSearchServerUrl", "http://localhost:9200"),
                }
            };
            
            var provider = new MemoryConfigurationProvider(configSource);

            return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
        }
    }
}
