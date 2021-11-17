using System;
using System.Data.Common;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Data;

namespace SFA.DAS.Reservations.Infrastructure.DatabaseInjection
{
    public static class DatabaseInjectionHelper
    {
        const string AzureResource = "https://database.windows.net/";

        public static ReservationsDataContext GetDataContext(string connectionString, string environmentName)
        {
            DbConnection connection = null;

            if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                connection = new SqlConnection(connectionString);
            }
            else
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                connection = new SqlConnection(connectionString)
                {
                    AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                };
            }

            var optionsBuilder = new DbContextOptionsBuilder<ReservationsDataContext>().UseSqlServer(connection);
            return new ReservationsDataContext(optionsBuilder.Options);
        }

    }
}
