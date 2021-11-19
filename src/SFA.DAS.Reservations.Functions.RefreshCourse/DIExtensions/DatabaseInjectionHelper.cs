using System;
using System.Data.Common;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Data;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.DIExtensions
{
    public static class AddDatabaseExtension
    {
        public static void AddDatabase(this IServiceCollection services, Domain.Configuration.ReservationsJobs config, string environmentName)
        {
            if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<ReservationsDataContext>(options => options.UseSqlServer(config.ConnectionString));
            }
            else
            {
                const string azureResource = "https://database.windows.net/";
                var azureServiceTokenProvider = new AzureServiceTokenProvider();

                var managedIdentitySqlConnection = new SqlConnection
                {
                    ConnectionString = config.ConnectionString,
                    AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(azureResource).Result
                };

                services.AddDbContext<ReservationsDataContext>(options => options.UseSqlServer(managedIdentitySqlConnection));
            }
        }
    }
}
