using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Reservations.Application.RefreshCourses.Handlers;
using SFA.DAS.Reservations.Application.RefreshCourses.Services;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Functions.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure.Configuration;
using SFA.DAS.Reservations.Infrastructure.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]

namespace SFA.DAS.Reservations.Functions.RefreshCourse
{
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
        private readonly ILoggerFactory _loggerFactory;
        public IConfiguration Configuration { get; }
        public ServiceProviderBuilder(ILoggerFactory loggerFactory)
        {
            var path = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot", EnvironmentVariableTarget.Process);
            if (!string.IsNullOrEmpty(path))
            {
                Directory.SetCurrentDirectory(path);
            }
            
            _loggerFactory = loggerFactory;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json")
                .AddEnvironmentVariables()
                .Build();
            
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) 
                .AddJsonFile("local.settings.json")
                .AddEnvironmentVariables()
                .AddAzureTableStorageConfiguration(
                    builder["ConfigurationStorageConnectionString"],
                    builder["ConfigNames"].Split(','),
                    builder["Environment"],
                    builder["Version"]
                )
                .Build();

            Configuration = config;
        }
        public IServiceProvider Build()
        {
            var services = new ServiceCollection();

            services.Configure<ReservationsJobs>(Configuration.GetSection("ReservationsJobs"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);

            var serviceProvider = services.BuildServiceProvider();

            var config = serviceProvider.GetService<ReservationsJobs>();

            services.AddSingleton(_ =>
                _loggerFactory.CreateLogger(LogCategories.CreateFunctionUserCategory("Common")));

            services.AddTransient<IStandardApiClient>(x => new StandardApiClient(config.ApprenticeshipBaseUrl));
            services.AddTransient<IFrameworkApiClient>(x => new FrameworkApiClient(config.ApprenticeshipBaseUrl));

            services.AddTransient<IApprenticeshipCourseService, ApprenticeshipCoursesService>();
            services.AddTransient<ICourseService, CourseService>();

            services.AddTransient<IGetCoursesHandler, GetCoursesHandler>();
            services.AddTransient<IStoreCourseHandler, StoreCourseHandler>();

            services.AddDbContext<ReservationsDataContext>(options => options.UseSqlServer(config.ConnectionString));
            services.AddScoped<IReservationsDataContext, ReservationsDataContext>(provider => provider.GetService<ReservationsDataContext>());
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

            services.AddTransient<ICourseRepository, CourseRepository>();

            return services.BuildServiceProvider();
        }
    }
}
