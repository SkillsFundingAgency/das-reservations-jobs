using System;
using System.Data;
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
                services
                    .AddDbContext<ReservationsDataContext>(options => options.UseInMemoryDatabase("SFA.DAS.Reservations")
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
            }
            else
            {
                const string AzureResource = "https://database.windows.net/";
                services.AddTransient<IDbConnection>(c =>
                {
                    // needs to be replaced
                    //var azureServiceTokenProvider = new AzureServiceTokenProvider();

                    return environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
                        ? new SqlConnection(config.ConnectionString)
                        : new SqlConnection
                        {
                            ConnectionString = config.ConnectionString,
                            //AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                        };

                });

                var option = new DbContextOptionsBuilder<ReservationsDataContext>();
                services.AddTransient<ReservationsDataContext>(provider => new ReservationsDataContext(option.Options, provider.GetService<IDbConnection>()));
            }

            services.AddScoped<IReservationsDataContext, ReservationsDataContext>(provider => provider.GetService<ReservationsDataContext>());
        }
    }
}
