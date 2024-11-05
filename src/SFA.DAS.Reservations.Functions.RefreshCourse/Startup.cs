using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Functions.RefreshCourse;
using SFA.DAS.Reservations.Functions.RefreshCourse.Extensions;
using SFA.DAS.Reservations.Infrastructure.Database;
using SFA.DAS.Reservations.Infrastructure.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]

namespace SFA.DAS.Reservations.Functions.RefreshCourse;

internal class Startup : IWebJobsStartup
{
    public void Configure(IWebJobsBuilder builder)
    {
        builder.AddExecutionContextBinding();
        builder.AddDependencyInjection<ServiceProviderBuilder>();
    }
}

internal class ServiceProviderBuilder : IServiceProviderBuilder
{
    private readonly IConfiguration _configuration;

    public ServiceProviderBuilder(IConfiguration configuration)
    {
        _configuration = configuration.BuildConfiguration();
    }

    public IServiceProvider Build()
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<IConfiguration>(_configuration);

        services.Configure<ReservationsJobs>(_configuration.GetSection("ReservationsJobs"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);
        
        var serviceProvider = services.BuildServiceProvider();

        var config = serviceProvider.GetService<ReservationsJobs>();
        
        services.AddHttpClient<IOuterApiClient, OuterApiClient>(client =>
        {
            var apimUrl = EnsureUrlEndWithForwardSlash(config.ReservationsApimUrl);
            client.BaseAddress = new Uri(apimUrl);
        });

        services.AddDasLogging(_configuration);

        services.AddDatabaseRegistration(config, _configuration["EnvironmentName"]);

        services.AddApplicationServices();

        services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();

        return services.BuildServiceProvider();
    }
    
    private static string EnsureUrlEndWithForwardSlash(string url)
    {
        return url.EndsWith('/') ? url : $"{url}/";
    }
}