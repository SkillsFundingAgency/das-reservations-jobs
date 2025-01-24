using System;
using System.Data;
using Azure.Core;
using Azure.Identity;
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
                    var azureServiceTokenProvider = CreateManagedIdentity();

                    return environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
                        ? new SqlConnection(config.ConnectionString)
                        : new SqlConnection
                        {
                            ConnectionString = config.ConnectionString,
                            AccessToken = azureServiceTokenProvider.GetTokenAsync(new TokenRequestContext(scopes: new string[] { AzureResource })).Result.Token
                        };

                });

                var option = new DbContextOptionsBuilder<ReservationsDataContext>();
                services.AddTransient<ReservationsDataContext>(provider => new ReservationsDataContext(option.Options, provider.GetService<IDbConnection>()));
            }

            services.AddScoped<IReservationsDataContext, ReservationsDataContext>(provider => provider.GetService<ReservationsDataContext>());
        }

        private static ChainedTokenCredential CreateManagedIdentity()
        {
            return new ChainedTokenCredential(
                new ManagedIdentityCredential(),
                new AzureCliCredential(),
                new VisualStudioCodeCredential(),
                new VisualStudioCredential());
        }
    }
}
