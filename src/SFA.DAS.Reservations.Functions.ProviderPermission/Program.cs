using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NServiceBus;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Application.ProviderPermissions.Handlers;
using SFA.DAS.Reservations.Application.ProviderPermissions.Service;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Functions.ProviderPermission;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.AzureSearch;
using SFA.DAS.Reservations.Infrastructure.Database;
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

        services.AddTransient<IProviderPermissionRepository, ProviderPermissionRepository>();

        services.AddTransient<IProviderPermissionsUpdatedHandler, ProviderPermissionsUpdatedHandler>();
        services.AddTransient<IUpdatedPermissionsEventValidator, UpdatedPermissionsEventValidator>();
        services.AddTransient<IProviderPermissionService, ProviderPermissionService>();
        
        services.AddAzureSearch();
        services.AddSingleton(new ReservationJobsEnvironment(configuration["EnvironmentName"]));

        services.AddDatabaseRegistration(config, configuration["EnvironmentName"]);
        services.AddHttpClient<IOuterApiClient, OuterApiClient>();

        services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();