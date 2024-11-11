using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Validators;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Reservations.Functions.LegalEntities;
using SFA.DAS.Reservations.Functions.LegalEntities.Extensions;
using SFA.DAS.Reservations.Infrastructure.Database;
using SFA.DAS.Reservations.Infrastructure.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]

namespace SFA.DAS.Reservations.Functions.LegalEntities;

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
        _configuration = configuration.BuildConfiguration();
    }

    public IServiceProvider Build()
    {
        var services = ServiceCollection ?? [];
        
        services.AddSingleton<IConfiguration>(_configuration);

        services.Configure<ReservationsJobs>(_configuration.GetSection("ReservationsJobs"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);

        var serviceProvider = services.BuildServiceProvider();

        var jobsConfig = serviceProvider.GetService<ReservationsJobs>();

        services.AddDasLogging(_configuration);

        services.AddDatabaseRegistration(jobsConfig, _configuration["EnvironmentName"]);
        services.AddHttpClient<IOuterApiClient, OuterApiClient>();

        services.AddApplicationServices();
       
        services.AddSingleton<IValidator<AddedLegalEntityEvent>, AddAccountLegalEntityValidator>();

        if (!_configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            services
                .AddApplicationInsightsTelemetryWorkerService()
                .ConfigureFunctionsApplicationInsights();
        }

        return services.BuildServiceProvider();
    }
}