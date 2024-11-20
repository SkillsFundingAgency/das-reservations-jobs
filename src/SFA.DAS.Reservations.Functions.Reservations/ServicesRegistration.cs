using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
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
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Api;
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
        services.Configure<ReservationsJobs>(configuration.GetSection("Encodings"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<EncodingConfig>>().Value);
        services.AddDasLogging(typeof(Program).Namespace);

        var config = configuration.GetSection("ReservationsJobs").Get<ReservationsJobs>();
        var environmentName = configuration["EnvironmentName"];

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

        services.AddTransient<INotificationsService, NotificationsService>();
        services.AddTransient<IEncodingService, EncodingService>();
        services.AddTransient<IReservationIndexRepository, ReservationIndexRepository>();
        services.AddTransient<IProviderPermissionRepository, ProviderPermissionRepository>();
        services.AddTransient<IAccountsService, AccountsService>();
        services.AddTransient<INotificationTokenBuilder, NotificationTokenBuilder>();

        services.AddSingleton<HttpClient>(x => x.GetService<IHttpClientFactory>().CreateClient());

        services.AddTransient<IAddNonLevyReservationToReservationsIndexAction, AddNonLevyReservationToReservationsIndexAction>();
        services.AddTransient<IIndexRegistry, IndexRegistry>();

        services.AddElasticSearch(config);
        services.AddSingleton(new ReservationJobsEnvironment(environmentName));
        services.AddDatabaseRegistration(config, environmentName);
        return services;
    }

    private string EnsureUrlEndWithForwardSlash(string url)
    {
        return url.EndsWith('/') ? url : $"{url}/";
    }
}