using System;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Data;

namespace SFA.DAS.Reservations.Infrastructure.Database
{
    public static class AddDatabaseExtension
    {
        public static void AddDatabaseRegistration(this IServiceCollection services, Domain.Configuration.ReservationsJobs config, string environmentName)
        {
            if (environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<ReservationsDataContext>(options => options.UseInMemoryDatabase("SFA.DAS.Reservations")
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
            }
            else if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
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
