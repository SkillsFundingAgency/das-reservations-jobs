using System;
using System.IO;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Reservations.Application.Accounts.Services;
using SFA.DAS.Reservations.Application.Providers.Services;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Functions.Reservations;
using SFA.DAS.Reservations.Functions.Reservations.Extensions;
using SFA.DAS.Reservations.Infrastructure.Api;
using SFA.DAS.Reservations.Infrastructure.Database;
using SFA.DAS.Reservations.Infrastructure.DependencyInjection;
using SFA.DAS.Reservations.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Infrastructure.Logging;

[assembly: WebJobsStartup(typeof(Startup))]

namespace SFA.DAS.Reservations.Functions.Reservations
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExecutionContextBinding();
            builder.AddDependencyInjection<ServiceProviderBuilder>();
            builder.AddExtension<NServiceBusExtensionConfig>();
        }
    }

    public class ServiceProviderBuilder : IServiceProviderBuilder
    {
        private const string EncodingConfigKey = "SFA.DAS.Encoding";
        public ServiceCollection ServiceCollection { get; set; }

        private readonly ILoggerFactory _loggerFactory;
        public IConfiguration Configuration { get; }

        public ServiceProviderBuilder(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _loggerFactory = loggerFactory;

            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", true)
                .AddEnvironmentVariables();
            if (!configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                config.AddAzureTableStorage(options =>
                    {
                        options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                        options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                        options.EnvironmentName = configuration["EnvironmentName"];
                        options.PreFixConfigurationKeys = false;
                        options.ConfigurationKeysRawJsonResult = new[] { EncodingConfigKey };
                    }
                );
            }

            Configuration = config.Build();
        }

        public IServiceProvider Build()
        {
            var services = ServiceCollection ?? new ServiceCollection();
            services.AddHttpClient();

            services.Configure<ReservationsJobs>(Configuration.GetSection("ReservationsJobs"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);

            services.Configure<AccountApiConfiguration>(Configuration.GetSection("AccountApiConfiguration"));
            services.AddSingleton<IAccountApiConfiguration>(cfg => cfg.GetService<IOptions<AccountApiConfiguration>>().Value);

            var serviceProvider = services.BuildServiceProvider();

            var jobsConfig = serviceProvider.GetService<ReservationsJobs>();

            if (!Configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                var encodingConfigJson = Configuration.GetSection(EncodingConfigKey).Value;
                var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
                services.AddSingleton(encodingConfig);
            }

            var nLogConfiguration = new NLogConfiguration();

            if (!Configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddLogging((options) =>
                {
                    options.SetMinimumLevel(LogLevel.Information);

                    options.AddConsole();
                    options.AddDebug();
                    nLogConfiguration.ConfigureNLog(Configuration);
                    options.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                });
            }

            services.AddTransient<IConfirmReservationHandler, ConfirmReservationHandler>();
            services.AddTransient<IApprenticeshipDeletedHandler, ApprenticeshipDeletedHandler>();
            services.AddTransient<INotifyEmployerOfReservationEventAction, NotifyEmployerOfReservationEventAction>();
            services.AddTransient<IReservationCreatedHandler, ReservationCreatedHandler>();
            services.AddTransient<IReservationDeletedHandler, ReservationDeletedHandler>();

            services.AddTransient<IReservationService, ReservationService>();

            services.AddHttpClient<IFindApprenticeshipTrainingService, FindApprenticeshipTrainingService>();
            services.AddTransient<IProviderService, ProviderService>();

            services.AddTransient<IReservationRepository, ReservationRepository>();
            services.AddTransient<IAccountRepository, AccountRepository>();

            if (!Configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddTransient<INotificationsService, NotificationsService>();
                services.AddTransient<IEncodingService, EncodingService>();
                services.AddTransient<IReservationIndexRepository, ReservationIndexRepository>();
                services.AddTransient<IProviderPermissionRepository, ProviderPermissionRepository>();
                services.AddTransient<IAccountsService, AccountsService>();
                services.AddTransient<INotificationTokenBuilder, NotificationTokenBuilder>();
            }

            services.AddTransient<IAddNonLevyReservationToReservationsIndexAction, AddNonLevyReservationToReservationsIndexAction>();

            services.AddTransient<IIndexRegistry, IndexRegistry>();

            services.AddElasticSearch(jobsConfig, Configuration["EnvironmentName"]);
            services.AddSingleton(new ReservationJobsEnvironment(Configuration["EnvironmentName"]));

            if (!Configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>();
                var newClient = clientFactory.CreateClient();
                services.AddSingleton(provider => newClient);
                services.AddTransient<IAccountApiClient, AccountApiClient>();
                services.AddTransient<INotificationsService, NotificationsService>();
            }
            
            services.AddNServiceBus();

            services.AddDatabaseRegistration(jobsConfig, Configuration["EnvironmentName"]);
            return services.BuildServiceProvider();
        }
    }
}