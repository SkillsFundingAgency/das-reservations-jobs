using System;
using System.IO;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Encoding;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Reservations.Application.Accounts.Services;
using SFA.DAS.Reservations.Application.OuterApi;
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

[assembly: WebJobsStartup(typeof(Startup))]
namespace SFA.DAS.Reservations.Functions.Reservations;

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
    private readonly IConfiguration _configuration;

    public ServiceCollection ServiceCollection { get; set; }

    public ServiceProviderBuilder(IConfiguration configuration)
    {
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

        _configuration = config.Build();
    }

    public IServiceProvider Build()
    {
        var services = ServiceCollection ?? new ServiceCollection();
        services.AddHttpClient();

        services.Configure<ReservationsJobs>(_configuration.GetSection("ReservationsJobs"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);

        var serviceProvider = services.BuildServiceProvider();

        var jobsConfig = serviceProvider.GetService<ReservationsJobs>();

        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
            builder.SetMinimumLevel(LogLevel.Trace);
        });

        var environmentName = _configuration["EnvironmentName"];

        if (!environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            var encodingConfigJson = _configuration.GetSection(EncodingConfigKey).Value;
            var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
            services.AddSingleton(encodingConfig);
        }

        services.AddTransient<IConfirmReservationHandler, ConfirmReservationHandler>();
        services.AddTransient<IApprenticeshipDeletedHandler, ApprenticeshipDeletedHandler>();
        services.AddTransient<INotifyEmployerOfReservationEventAction, NotifyEmployerOfReservationEventAction>();
        services.AddTransient<IReservationCreatedHandler, ReservationCreatedHandler>();
        services.AddTransient<IReservationDeletedHandler, ReservationDeletedHandler>();

        services.AddTransient<IReservationService, ReservationService>();

        services.AddTransient<IFindApprenticeshipTrainingService, FindApprenticeshipTrainingService>();
        services.AddTransient<IProviderService, ProviderService>();

        services.AddHttpClient<IOuterApiClient, OuterApiClient>(client =>
        {
            var apimUrl = EnsureUrlEndWithForwardSlash(jobsConfig.ReservationsApimUrl);
            client.BaseAddress = new Uri(apimUrl);
        });

        services.AddTransient<IReservationRepository, ReservationRepository>();
        services.AddTransient<IAccountRepository, AccountRepository>();
        
        if (!environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddTransient<INotificationsService, NotificationsService>();
            services.AddTransient<IEncodingService, EncodingService>();
            services.AddTransient<IReservationIndexRepository, ReservationIndexRepository>();
            services.AddTransient<IProviderPermissionRepository, ProviderPermissionRepository>();
            services.AddTransient<IAccountsService, AccountsService>();
            services.AddTransient<INotificationTokenBuilder, NotificationTokenBuilder>();

            var clientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var newClient = clientFactory.CreateClient();
            services.AddSingleton(provider => newClient);
        }

        services.AddTransient<IAddNonLevyReservationToReservationsIndexAction, AddNonLevyReservationToReservationsIndexAction>();
        services.AddTransient<IIndexRegistry, IndexRegistry>();
        
        services.AddElasticSearch(jobsConfig);
        services.AddSingleton(new ReservationJobsEnvironment(environmentName));

        services.AddNServiceBus(environmentName);
        services.AddDatabaseRegistration(jobsConfig, environmentName);

        return services.BuildServiceProvider();
    }
    
    private static string EnsureUrlEndWithForwardSlash(string url)
    {
        return url.EndsWith('/') ? url : $"{url}/";
    }
}