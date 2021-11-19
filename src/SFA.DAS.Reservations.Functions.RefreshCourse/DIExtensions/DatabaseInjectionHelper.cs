using System;
using System.Data.Common;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.DIExtensions
{
    public static class AddDatabaseExtension
    {
        public static void AddDatabase(this IServiceCollection services, IServiceProvider serviceProvider, ReservationsJobs config, string environmentName)
        {
            if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<ReservationsDataContext>(options => options.UseSqlServer(config.ConnectionString));
            }
            else
            {
                services.AddDbContext<ReservationsDataContext>(x => new ReservationsDataContext(
                    serviceProvider.GetService<IConfiguration>(),
                    config,
                    new DbContextOptions<ReservationsDataContext>(),
                    serviceProvider.GetService<AzureServiceTokenProvider>()), ServiceLifetime.Transient);
            }
        }
    }
}
