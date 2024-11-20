using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Infrastructure.Database;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Functions.ProviderPermission;
using SFA.DAS.Reservations.Infrastructure.ElasticSearch;
using NServiceBus;
using SFA.DAS.Reservations.Functions.LegalEntities;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.NServiceBus;

[assembly: NServiceBusTriggerFunction(AzureFunctionsQueueNames.ProviderPermissionQueue)]

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(builder => builder.BuildDasConfiguration())
    .ConfigureNServiceBus(AzureFunctionsQueueNames.ProviderPermissionQueue)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), configuration));

        services.AddOptions();

        services.Configure<ReservationsJobs>(configuration.GetSection("ReservationsJobs"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);

        var config = configuration.GetSection("ReservationsJobs").Get<ReservationsJobs>();
        services.AddDasLogging(typeof(Program).Namespace);

        services.AddTransient<IReservationIndexRefreshHandler, ReservationIndexRefreshHandler>();
        services.AddTransient<IReservationService, ReservationService>();
        services.AddTransient<IReservationRepository, ReservationRepository>();
        services.AddTransient<IReservationIndexRepository, ReservationIndexRepository>();
        services.AddTransient<IProviderPermissionRepository, ProviderPermissionRepository>();
        services.AddTransient<IIndexRegistry, IndexRegistry>();

        services.AddElasticSearch(config);
        services.AddSingleton(new ReservationJobsEnvironment(configuration["EnvironmentName"]));

        services.AddDatabaseRegistration(config, configuration["EnvironmentName"]);
        services.AddHttpClient<IOuterApiClient, OuterApiClient>();

        services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();