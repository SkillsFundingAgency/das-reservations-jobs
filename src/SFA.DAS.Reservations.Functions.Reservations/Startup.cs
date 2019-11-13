using System;
using System.IO;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Reservations.Application.Accounts.Services;
using SFA.DAS.Reservations.Application.Providers.Services;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Functions.Reservations;
using SFA.DAS.Reservations.Infrastructure.Configuration;
using SFA.DAS.Reservations.Infrastructure.DependencyInjection;
using SFA.DAS.Reservations.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Infrastructure.Logging;

[assembly: WebJobsStartup(typeof(Startup))]

namespace SFA.DAS.Reservations.Functions.Reservations
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
                .AddJsonFile("local.settings.json",true)
                .AddEnvironmentVariables()
                .AddAzureTableStorageConfiguration(
                    configuration["ConfigurationStorageConnectionString"],
                    configuration["ConfigNames"].Split(','),
                    configuration["EnvironmentName"],
                    configuration["Version"])
                .Build();

            Configuration = config;
        }

        public IServiceProvider Build()
        {

            var services = new ServiceCollection();
            services.AddHttpClient();

            services.Configure<ReservationsJobs>(Configuration.GetSection("ReservationsJobs"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);
            
            services.Configure<AccountApiConfiguration>(Configuration.GetSection("AccountApiConfiguration"));
            services.AddSingleton<IAccountApiConfiguration>(cfg =>  cfg.GetService<IOptions<AccountApiConfiguration>>().Value);

            services.Configure<NotificationsApiClientConfiguration>(Configuration.GetSection("NotificationsApi"));
            services.AddSingleton<INotificationsApiClientConfiguration>(cfg => cfg.GetService<IOptions<NotificationsApiClientConfiguration>>().Value);

            var serviceProvider = services.BuildServiceProvider();

            var jobsConfig = serviceProvider.GetService<ReservationsJobs>();

            var encodingConfigJson = Configuration.GetSection(nameof(EncodingConfig)).Value;
            var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
            services.AddSingleton(encodingConfig);

            var nLogConfiguration = new NLogConfiguration();

            services.AddLogging((options) =>
            {
                options.AddConfiguration(Configuration.GetSection("Logging"));
                options.SetMinimumLevel(LogLevel.Trace);
                options.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
                options.AddConsole();
                options.AddDebug();
                nLogConfiguration.ConfigureNLog(Configuration);
            });

            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddSingleton(_ => _loggerFactory.CreateLogger(LogCategories.CreateFunctionUserCategory("Common")));

            services.AddTransient<IConfirmReservationHandler,ConfirmReservationHandler>();
            services.AddTransient<IApprenticeshipDeletedHandler,ApprenticeshipDeletedHandler>();
            services.AddTransient<INotifyEmployerOfReservationEventAction, NotifyEmployerOfReservationEventAction>();

            services.AddTransient<IReservationService,ReservationService>();
            services.AddTransient<IProviderService, ProviderService>();
            services.AddTransient<IAccountsService, AccountsService>();
            services.AddTransient<INotificationsService, NotificationsService>();

            services.AddTransient<INotificationTokenBuilder, NotificationTokenBuilder>();
            services.AddTransient<IReservationRepository,ReservationRepository>();
            services.AddTransient<IEncodingService, EncodingService>();
            services.AddTransient<IAddNonLevyReservationToReservationsIndexAction, AddNonLevyReservationToReservationsIndexAction>();

            services.AddTransient<IReservationIndexRepository, ReservationIndexRepository>();
            services.AddTransient<IProviderPermissionRepository, ProviderPermissionRepository>();

            services.AddTransient<IIndexRegistry, IndexRegistry>();

            services.AddElasticSearch(jobsConfig, Configuration["EnvironmentName"]);
            services.AddSingleton(new ReservationJobsEnvironment(Configuration["EnvironmentName"]));

            var clientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var newClient = clientFactory.CreateClient();
            services.AddSingleton(provider => newClient);
            services.AddTransient<IProviderApiClient>(provider => new ProviderApiClient(jobsConfig.ApprenticeshipBaseUrl));
            services.AddTransient<IAccountApiClient, AccountApiClient>();
            services.AddHttpClient<INotificationsService, NotificationsService>(
                client =>
                {
                    var notificationsConfig = serviceProvider.GetService<INotificationsApiClientConfiguration>();
                    var bearerToken = (IGenerateBearerToken)new JwtBearerTokenGenerator(notificationsConfig);
                    client.BaseAddress = new Uri(notificationsConfig.ApiBaseUrl);
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken.Generate().Result);
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                });

            services.AddDbContext<ReservationsDataContext>(options => options.UseSqlServer(jobsConfig.ConnectionString));
            services.AddScoped<IReservationsDataContext, ReservationsDataContext>(provider => provider.GetService<ReservationsDataContext>());

            return services.BuildServiceProvider();
        }
    }
}
