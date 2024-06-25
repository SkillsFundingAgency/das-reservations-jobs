using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
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
    public ServiceCollection ServiceCollection { get; set; }

    private readonly IConfiguration _configuration;

    public ServiceProviderBuilder(IConfiguration configuration)
    {
        _configuration = configuration.BuildDasConfiguration();
    }

    public IServiceProvider Build()
    {
        var services = ServiceCollection ?? new ServiceCollection();
        services.AddHttpClient();

        services.Configure<ReservationsJobs>(_configuration.GetSection("ReservationsJobs"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);

        services.Configure<AccountApiConfiguration>(_configuration.GetSection("AccountApiConfiguration"));
        services.AddSingleton<IAccountApiConfiguration>(cfg => cfg.GetService<IOptions<AccountApiConfiguration>>().Value);

        var serviceProvider = services.BuildServiceProvider();

        var jobsConfig = serviceProvider.GetService<ReservationsJobs>();

        if (!_configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            var encodingConfigJson = _configuration.GetSection(ConfigKeys.Encoding).Value;
            var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
            services.AddSingleton(encodingConfig);
        }

        var nLogConfiguration = new NLogConfiguration();

        if (!_configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddLogging(options =>
            {
                options.SetMinimumLevel(LogLevel.Information);

                options.AddConsole();
                options.AddDebug();
                nLogConfiguration.ConfigureNLog(_configuration);
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

        if (!_configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
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

        services.AddElasticSearch(jobsConfig, _configuration["EnvironmentName"]);
        services.AddSingleton(new ReservationJobsEnvironment(_configuration["EnvironmentName"]));

        if (!_configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            var clientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var newClient = clientFactory.CreateClient();
            services.AddSingleton(provider => newClient);
            services.AddTransient<IAccountApiClient, AccountApiClient>();
        }

        services.AddDatabaseRegistration(jobsConfig, _configuration["EnvironmentName"]);
        return services.BuildServiceProvider();
    }
}