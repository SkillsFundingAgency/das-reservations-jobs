using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
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
    public class ReservationsDataContext :DbContext, IReservationsDataContext
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

        public ReservationsDataContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public ReservationsDataContext(DbContextOptions options) : base(options)
        {
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