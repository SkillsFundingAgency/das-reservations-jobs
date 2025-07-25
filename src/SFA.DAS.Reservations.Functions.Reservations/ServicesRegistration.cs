using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Encoding;
using SFA.DAS.Reservations.Application.Accounts.Services;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Application.Providers.Services;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Api;
using SFA.DAS.Reservations.Infrastructure.AzureSearch;
using SFA.DAS.Reservations.Infrastructure.Database;
using SFA.DAS.Reservations.Infrastructure.ElasticSearch;
using ServiceDescriptor = Microsoft.Extensions.DependencyInjection.ServiceDescriptor;

namespace SFA.DAS.Reservations.Functions.Reservations;

public class ServicesRegistration(IServiceCollection services, IConfiguration configuration)
{
    public IServiceCollection Register()
    {
        services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), configuration));

        services.AddOptions();

        services.Configure<ReservationsJobs>(configuration.GetSection("ReservationsJobs"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);
        services.AddDasLogging(typeof(Program).Namespace);

        var config = configuration.GetSection("ReservationsJobs").Get<ReservationsJobs>();
        var environmentName = configuration["EnvironmentName"];

        if (!environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            var encodingConfigJson = configuration.GetSection("SFA.DAS.Encoding").Value;
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

        services.AddHttpClient<IOuterApiClient, OuterApiClient>();

        services.AddTransient<IReservationRepository, ReservationRepository>();
        services.AddTransient<IAccountRepository, AccountRepository>();

        services.AddTransient<IEncodingService, EncodingService>();
        services.AddTransient<IElasticReservationIndexRepository, ElasticReservationIndexRepository>();
        services.AddTransient<IProviderPermissionRepository, ProviderPermissionRepository>();
        services.AddTransient<IAccountsService, AccountsService>();
        services.AddTransient<INotificationTokenBuilder, NotificationTokenBuilder>();

        services.AddSingleton<HttpClient>(x => x.GetService<IHttpClientFactory>().CreateClient());

        services.AddTransient<IAddNonLevyReservationToReservationsIndexAction, AddNonLevyReservationToReservationsIndexAction>();
        services.AddTransient<IIndexRegistry, IndexRegistry>();

        services.AddElasticSearch(config);
        services.AddAzureSearch();
        services.AddSingleton(new ReservationJobsEnvironment(environmentName));
        services.AddDatabaseRegistration(config, environmentName);
        return services;
    }
}