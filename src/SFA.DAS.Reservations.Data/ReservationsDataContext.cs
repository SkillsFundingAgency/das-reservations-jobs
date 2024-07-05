using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

        private readonly IDbConnection _connection;


        public ReservationsDataContext()
        {
        }

        public ReservationsDataContext(DbContextOptions options) : base(options)
        {
        }

        public ReservationsDataContext(DbContextOptions<ReservationsDataContext> options, IDbConnection connection) : base(options)
        {
            _connection = connection;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connection != null)
            {
                optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseSqlServer(_connection as DbConnection, options => options.EnableRetryOnFailure(3));
            }

            optionsBuilder.UseLazyLoadingProxies();
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