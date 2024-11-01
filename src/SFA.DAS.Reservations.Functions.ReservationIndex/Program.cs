using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Infrastructure.Database;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Functions.ReservationIndex;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(builder => builder.BuildDasConfiguration())
    .ConfigureServices((context, services) =>
    {
        //services.AddApplicationInsightsTelemetryWorkerService();
        //services.ConfigureFunctionsApplicationInsights();

        var configuration = context.Configuration;
        services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), configuration));

        services.AddOptions();

        services.Configure<ReservationsJobs>(configuration.GetSection("ReservationsJobs"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);

        var config = configuration.GetSection("ReservationsJobs").Get<ReservationsJobs>();
        services.AddDasLogging();

        services.AddTransient<IReservationIndexRefreshHandler, ReservationIndexRefreshHandler>();
        services.AddTransient<IReservationService, ReservationService>();
        services.AddTransient<IReservationRepository, ReservationRepository>();
        services.AddTransient<IReservationIndexRepository, ReservationIndexRepository>();
        services.AddTransient<IProviderPermissionRepository, ProviderPermissionRepository>();
        services.AddTransient<IIndexRegistry, IndexRegistry>();

        //services.AddElasticSearch(config);
        services.AddSingleton(new ReservationJobsEnvironment(configuration["EnvironmentName"]));

        services.AddDatabaseRegistration(config, configuration["EnvironmentName"]);

        //services.AddTransient<IFindApprenticeshipTrainingService, FindApprenticeshipTrainingService>();
        //services.AddTransient<IApprenticeshipCourseService, ApprenticeshipCoursesService>();
        //services.AddTransient<ICourseService, CourseService>();

        //services.AddTransient<IGetCoursesHandler, GetCoursesHandler>();
        //services.AddTransient<IStoreCourseHandler, StoreCourseHandler>();

        //services.AddDatabaseRegistration(config, configuration["EnvironmentName"]);

        //services.AddTransient<ICourseRepository, CourseRepository>();
        services.AddHttpClient<IOuterApiClient, OuterApiClient>();


    })
    .Build();

host.Run();