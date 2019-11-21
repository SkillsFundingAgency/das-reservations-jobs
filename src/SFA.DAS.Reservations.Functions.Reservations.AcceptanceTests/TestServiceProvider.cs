using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests
{
    public class TestServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public TestServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            var configuration = GenerateConfiguration();

            var serviceProviderBuilder = new ServiceProviderBuilder(new LoggerFactory(), configuration)
            {
                ServiceCollection = serviceCollection
            };
            
            var encodingService = new Mock<IEncodingService>();
            encodingService.Setup(x => x.Decode(It.Is<string>(s => s.Equals(TestDataValues.NonLevyHashedAccountId)),It.IsAny<EncodingType>())).Returns(TestDataValues.NonLevyAccountId);
            encodingService.Setup(x => x.Encode(It.Is<long>(l => l.Equals(TestDataValues.NonLevyAccountId)),It.IsAny<EncodingType>())).Returns(TestDataValues.NonLevyHashedAccountId);
            encodingService.Setup(x => x.Decode(It.Is<string>(s => s.Equals(TestDataValues.LevyHashedAccountId)),It.IsAny<EncodingType>())).Returns(TestDataValues.LevyAccountId);
            encodingService.Setup(x => x.Encode(It.Is<long>(l => l.Equals(TestDataValues.LevyAccountId)),It.IsAny<EncodingType>())).Returns(TestDataValues.LevyHashedAccountId);
            serviceCollection.AddSingleton(encodingService.Object);
            
            var mockProviderApiClient = new Mock<IProviderApiClient>();
            serviceCollection.AddSingleton(mockProviderApiClient.Object);

            var mockAccountApiClient = new Mock<IAccountApiClient>();
            serviceCollection.AddSingleton(mockAccountApiClient.Object);

            var mockNotificationService = new Mock<INotificationsService>();
            serviceCollection.AddSingleton(mockNotificationService.Object);

            var mockReservationIndex = new Mock<IReservationIndexRepository>();
            serviceCollection.AddSingleton(mockReservationIndex.Object);
             
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
                    new KeyValuePair<string, string>("ConfigNames", "SFA.DAS.Reservations.Jobs"),
                    new KeyValuePair<string, string>("EnvironmentName", "DEV"),
                    new KeyValuePair<string, string>("Version", "1.0"),
                    new KeyValuePair<string, string>("ReservationsJobs:ElasticSearchServerUrl", "http://localhost:9200"),
                }
            };
            
            var provider = new MemoryConfigurationProvider(configSource);

            return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
        }
    }
}
