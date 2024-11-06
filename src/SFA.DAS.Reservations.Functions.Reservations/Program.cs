using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Infrastructure.Database;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using SFA.DAS.Encoding;
using SFA.DAS.Reservations.Application.Accounts.Services;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Application.Providers.Services;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Functions.Reservations;
using SFA.DAS.Reservations.Functions.Reservations.Extensions;
using SFA.DAS.Reservations.Infrastructure.Api;
using SFA.DAS.Reservations.Infrastructure.ElasticSearch;
using NServiceBus;

[assembly: NServiceBusTriggerFunction("SFA.DAS.Reservations.Functions.Reservations")]

const string EncodingConfigKey = "SFA.DAS.Encoding";

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(builder => builder.BuildDasConfiguration())
    .ConfigureNServiceBus()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), configuration));

        services.AddOptions();

        services.Configure<ReservationsJobs>(configuration.GetSection("ReservationsJobs"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);

        var config = configuration.GetSection("ReservationsJobs").Get<ReservationsJobs>();
        services.AddDasLogging();

        var environmentName = configuration["EnvironmentName"];

        if (!environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            var encodingConfigJson = configuration.GetSection(EncodingConfigKey).Value;
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
            var apimUrl = EnsureUrlEndWithForwardSlash(config.ReservationsApimUrl);
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

            //var clientFactory = services.GetService<IHttpClientFactory>();
            //var newClient = clientFactory.CreateClient();
            //services.AddSingleton(provider => newClient);
        }

        services.AddTransient<IAddNonLevyReservationToReservationsIndexAction, AddNonLevyReservationToReservationsIndexAction>();
        services.AddTransient<IIndexRegistry, IndexRegistry>();

        services.AddElasticSearch(config);
        services.AddSingleton(new ReservationJobsEnvironment(environmentName));

        //services.AddNServiceBus(environmentName);
        services.AddDatabaseRegistration(config, environmentName);







        //services.AddDatabaseRegistration(config, configuration["EnvironmentName"]);

        //services.AddTransient<IAzureQueueService, AzureQueueService>();
        //services.AddTransient<IAccountLegalEntitiesService, AccountLegalEntitiesService>();
        //services.AddTransient<IAccountsService, AccountsService>();

        //services.AddHttpClient<IOuterApiClient, OuterApiClient>();

        //services.AddTransient<IAccountLegalEntityRepository, AccountLegalEntityRepository>();
        //services.AddTransient<IAccountRepository, AccountRepository>();

        //services.AddTransient<IAddAccountLegalEntityHandler, AddAccountLegalEntityHandler>();
        //services.AddTransient<IRemoveLegalEntityHandler, RemoveLegalEntityHandler>();
        //services.AddTransient<ISignedLegalAgreementHandler, SignedLegalAgreementHandler>();
        //services.AddTransient<ILevyAddedToAccountHandler, LevyAddedToAccountHandler>();
        //services.AddTransient<IAddAccountHandler, AddAccountHandler>();
        //services.AddTransient<IAccountNameUpdatedHandler, AccountNameUpdatedHandler>();

        //services.AddSingleton<IValidator<AddedLegalEntityEvent>, AddAccountLegalEntityValidator>();
    })
    .Build();

host.Run();


string EnsureUrlEndWithForwardSlash(string url)
{
    return url.EndsWith('/') ? url : $"{url}/";
}