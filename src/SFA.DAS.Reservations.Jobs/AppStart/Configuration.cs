using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Application.QueueMonitoring.Handlers;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.AzureServiceBus;
using SFA.DAS.Reservations.Infrastructure.Caching;
using SFA.DAS.Reservations.Infrastructure.Configuration;
using SFA.DAS.Reservations.Infrastructure.ExternalMessagePublisher;
using SFA.DAS.Reservations.Jobs.Jobs;

namespace SFA.DAS.Reservations.Jobs.AppStart
{
    public static class Configuration
    {
        public static void AddServiceConfiguration(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .Build();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .AddAzureTableStorageConfiguration(
                    builder["ConfigurationStorageConnectionString"],
                    builder["ConfigNames"].Split(','),
                    builder["EnvironmentName"],
                    builder["Version"])
                .Build();

            services.Configure<ReservationsJobs>(config.GetSection("ReservationsJobs"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);
            services.AddSingleton<ICheckAvailableQueuesHealth, CheckAvailableQueuesHealth>();
            services.AddTransient<IAzureQueueService, AzureQueueService>();
            services.AddTransient<IExternalMessagePublisher, SlackMessagePublisher>();
            services.AddTransient<ICacheStorageService, CacheStorageService>();
            services.AddDistributedMemoryCache();
            services.AddSingleton<CheckQueueHealthJob>();
            services.AddSingleton(new JobEnvironment(config["EnvironmentName"]));
            services.BuildServiceProvider();
        }
    }
}
