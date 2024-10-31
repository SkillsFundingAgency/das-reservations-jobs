using System;
using System.IO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Reservations.Application.ProviderPermissions.Handlers;
using SFA.DAS.Reservations.Application.ProviderPermissions.Service;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Functions.ProviderPermission;
using SFA.DAS.Reservations.Infrastructure.AzureServiceBus;
using SFA.DAS.Reservations.Infrastructure.Database;
using SFA.DAS.Reservations.Infrastructure.DependencyInjection;
using SFA.DAS.Reservations.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Infrastructure.Logging;

[assembly: WebJobsStartup(typeof(Startup))]
namespace SFA.DAS.Reservations.Functions.ProviderPermission
{
    internal class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExecutionContextBinding();
            builder.AddDependencyInjection<ServiceProviderBuilder>();
            builder.AddExtension<NServiceBusExtensionConfig>();
        }
    }
    internal class ServiceProviderBuilder : IServiceProviderBuilder
    {
        private readonly ILoggerFactory _loggerFactory;
        public IConfiguration Configuration { get; }
        public ServiceProviderBuilder(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _loggerFactory = loggerFactory;

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", true)
                .AddEnvironmentVariables();
            if (!configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(',');
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                });
            }
            

            Configuration = config.Build();
        }

        public IServiceProvider Build()
        {
            var services = new ServiceCollection();

            services.Configure<ReservationsJobs>(Configuration.GetSection("ReservationsJobs"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);

            var serviceProvider = services.BuildServiceProvider();

            var config = serviceProvider.GetService<ReservationsJobs>();

            var nLogConfiguration = new NLogConfiguration();

            services.AddLogging(builder =>
            {
                builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
                builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
                
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
                builder.AddConsole();
                builder.AddDebug();

                nLogConfiguration.ConfigureNLog(Configuration);
            });

            services.AddDatabaseRegistration(config, Configuration["EnvironmentName"]);

            services.AddTransient<IAzureQueueService, AzureQueueService>();
            services.AddTransient<IReservationService, ReservationService>();
            services.AddTransient<IProviderPermissionService, ProviderPermissionService>();
            
            services.AddTransient<IProviderPermissionRepository, ProviderPermissionRepository>();
            services.AddTransient<IReservationRepository, ReservationRepository>();
            services.AddTransient<IReservationIndexRepository, ReservationIndexRepository>();
            services.AddTransient<IIndexRegistry, IndexRegistry>();

            services.AddElasticSearch(config);

            services.AddSingleton(new ReservationJobsEnvironment(Configuration["EnvironmentName"]));
            services.AddTransient<IProviderPermissionsUpdatedHandler, ProviderPermissionsUpdatedHandler>();
            services.AddTransient<IUpdatedPermissionsEventValidator, UpdatedPermissionsEventValidator>();
            services.AddLogging();
            
            services
                .AddApplicationInsightsTelemetryWorkerService()
                .ConfigureFunctionsApplicationInsights();
            
            return services.BuildServiceProvider();
        }
    }
}
