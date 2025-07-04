﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.Encoding;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Domain.Reservations;
using Azure.Search.Documents.Indexes.Models;
using Azure;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests
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
            encodingService.Setup(x => x.Decode(It.Is<string>(s => s.Equals(TestDataValues.NonLevyHashedAccountId)), It.IsAny<EncodingType>())).Returns(TestDataValues.NonLevyAccountId);
            encodingService.Setup(x => x.Encode(It.Is<long>(l => l.Equals(TestDataValues.NonLevyAccountId)), It.IsAny<EncodingType>())).Returns(TestDataValues.NonLevyHashedAccountId);
            encodingService.Setup(x => x.Decode(It.Is<string>(s => s.Equals(TestDataValues.LevyHashedAccountId)), It.IsAny<EncodingType>())).Returns(TestDataValues.LevyAccountId);
            encodingService.Setup(x => x.Encode(It.Is<long>(l => l.Equals(TestDataValues.LevyAccountId)), It.IsAny<EncodingType>())).Returns(TestDataValues.LevyHashedAccountId);
            serviceCollection.AddSingleton(encodingService.Object);

            var findApprenticeshipTrainingService = new Mock<IFindApprenticeshipTrainingService>();
            serviceCollection.AddSingleton(findApprenticeshipTrainingService.Object);

            var mockElasticReservationIndex = new Mock<IElasticReservationIndexRepository>();
            serviceCollection.AddSingleton(mockElasticReservationIndex.Object);

            var mockAzureSearchReservationIndex = new Mock<IAzureSearchReservationIndexRepository>();
            serviceCollection.AddSingleton(mockAzureSearchReservationIndex.Object);

            var mockProviderPermissions = new Mock<IProviderPermissionRepository>();
            serviceCollection.AddSingleton(mockProviderPermissions.Object);

            var mockAccountsService = new Mock<IAccountsService>();
            serviceCollection.AddSingleton(mockAccountsService.Object);

            var mockNotificationTokenBuilder = new Mock<INotificationTokenBuilder>();
            serviceCollection.AddSingleton(mockNotificationTokenBuilder.Object);

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
                    new KeyValuePair<string, string>("ReservationsJobs:AzureSearchBaseUrl", "https://localhost:9301")
                ]
            };

            var provider = new MemoryConfigurationProvider(configSource);

            return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
        }
    }
}
