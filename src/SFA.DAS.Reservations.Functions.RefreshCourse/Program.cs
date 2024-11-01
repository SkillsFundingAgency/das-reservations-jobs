using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Reservations.Application.RefreshCourses.Handlers;
using SFA.DAS.Reservations.Application.RefreshCourses.Services;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Functions.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure.Api;
using SFA.DAS.Reservations.Infrastructure.Database;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(builder => builder.BuildDasConfiguration())
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var configuration = context.Configuration;
        services.Configure<ReservationsJobs>(configuration.GetSection("ReservationsJobs"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);

        var config = configuration.GetSection("ReservationsJobs").Get<ReservationsJobs>();
        services.AddDasLogging();

        services.AddTransient<IFindApprenticeshipTrainingService, FindApprenticeshipTrainingService>();
        services.AddTransient<IApprenticeshipCourseService, ApprenticeshipCoursesService>();
        services.AddTransient<ICourseService, CourseService>();

        services.AddTransient<IGetCoursesHandler, GetCoursesHandler>();
        services.AddTransient<IStoreCourseHandler, StoreCourseHandler>();

        services.AddDatabaseRegistration(config, configuration["EnvironmentName"]);

        services.AddTransient<ICourseRepository, CourseRepository>();

    })
    .Build();

host.Run();