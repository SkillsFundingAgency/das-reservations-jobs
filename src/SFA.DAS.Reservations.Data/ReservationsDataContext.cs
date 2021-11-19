using System;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data
{
    public interface IReservationsDataContext
    {
        DbSet<Reservation> Reservations { get; set; }
        DbSet<Course> Apprenticeships { get; set; }
        DbSet<AccountLegalEntity> AccountLegalEntities { get; set; }
        DbSet<ProviderPermission> ProviderPermissions { get; set; }
        DbSet<Account> Accounts { get; set; }
        DatabaseFacade Database { get; }
        int SaveChanges();
    }
    public class ReservationsDataContext : DbContext, IReservationsDataContext
    {
        public override DatabaseFacade Database
        {
            get { return base.Database; }
        }

        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Course> Apprenticeships { get; set; }
        public DbSet<AccountLegalEntity> AccountLegalEntities { get; set; }
        public DbSet<ProviderPermission> ProviderPermissions { get; set; }

        public DbSet<Account> Accounts { get; set; }

        private readonly IConfiguration _configuration;
        private readonly AzureServiceTokenProvider _azureServiceTokenProvider;
        private ReservationsJobs _reservationsJobsConfig;

        public ReservationsDataContext()
        {
        }

        public ReservationsDataContext(DbContextOptions options) : base(options)
        {
        }

        public ReservationsDataContext(IConfiguration configuration, ReservationsJobs jobsConfig, DbContextOptions options, AzureServiceTokenProvider azureServiceTokenProvider) : base(options)
        {
            _reservationsJobsConfig = jobsConfig;
            _azureServiceTokenProvider = azureServiceTokenProvider;
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_configuration == null || _azureServiceTokenProvider == null)
            {
                return;
            }

            const string azureResource = "https://database.windows.net/";

            var connection = new SqlConnection
            {
                ConnectionString = _reservationsJobsConfig.ConnectionString,
                AccessToken = _azureServiceTokenProvider.GetAccessTokenAsync(azureResource).Result,
            };

            optionsBuilder.UseSqlServer(connection, options =>
                options.EnableRetryOnFailure(
                    5,
                    TimeSpan.FromSeconds(20),
                    null
                ));
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configuration.Course());
            modelBuilder.ApplyConfiguration(new Configuration.Reservation());
            modelBuilder.ApplyConfiguration(new Configuration.AccountLegalEntity());
            modelBuilder.ApplyConfiguration(new Configuration.ProviderPermission());
            modelBuilder.ApplyConfiguration(new Configuration.Account());

            base.OnModelCreating(modelBuilder);
        }
    }
}