﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Encoding;
using SFA.DAS.Reservations.Application.Reservations.Services;

namespace SFA.DAS.Reservations.Functions.LegalEntities.AcceptanceTests
{
    public class TestServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public TestServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            var configuration = GenerateConfiguration();

            var servicesRegistration = new ServicesRegistration(serviceCollection, configuration);
            servicesRegistration.Register();

            var encodingService = new Mock<IEncodingService>();

            encodingService.Setup(x => x.Decode(It.Is<string>(s => s.Equals(TestDataValues.NonLevyHashedAccountId)),
                It.Is<EncodingType>(t => t == EncodingType.PublicAccountId)))
                .Returns(TestDataValues.NonLevyAccountId);

            encodingService.Setup(x => x.Encode(It.Is<long>(l => l.Equals(TestDataValues.NonLevyAccountId)),
                It.Is<EncodingType>(t => t == EncodingType.PublicAccountId)))
                .Returns(TestDataValues.NonLevyHashedAccountId);

            encodingService.Setup(x => x.Decode(It.Is<string>(s => s.Equals(TestDataValues.LevyHashedAccountId)),
                It.Is<EncodingType>(t => t == EncodingType.PublicAccountId)))
                .Returns(TestDataValues.LevyAccountId);

            encodingService.Setup(x => x.Encode(It.Is<long>(l => l.Equals(TestDataValues.LevyAccountId)),
                It.Is<EncodingType>(t => t == EncodingType.PublicAccountId)))
                .Returns(TestDataValues.LevyHashedAccountId);

            serviceCollection.AddSingleton(encodingService.Object);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        private static IConfigurationRoot GenerateConfiguration()
        {
            var configSource = new MemoryConfigurationSource
            {
                InitialData =
                [
                    new KeyValuePair<string, string>("ConfigurationStorageConnectionString", "UseDevelopmentStorage=true;"),
                    new KeyValuePair<string, string>("ConfigNames", "SFA.DAS.Reservations.Jobs"),
                    new KeyValuePair<string, string>("EnvironmentName", "DEV"),
                    new KeyValuePair<string, string>("Version", "1.0"),
                    new KeyValuePair<string, string>("ReservationsJobs:ElasticSearchServerUrl", "http://localhost:9200"),
                    new KeyValuePair<string, string>("ReservationsJobs:ReservationsApimUrl", "http://localhost:9201"),
                    new KeyValuePair<string, string>("ReservationsJobs:AzureSearchBaseUrl", "https://localhost:9301")
                ]
            };

            var provider = new MemoryConfigurationProvider(configSource);

            return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
        }
    }
}
