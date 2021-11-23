using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data
{
    public class TestReservationsDataContext : DbContext, IReservationsDataContext
    {

        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Course> Apprenticeships { get; set; }
        public DbSet<AccountLegalEntity> AccountLegalEntities { get; set; }
        public DbSet<ProviderPermission> ProviderPermissions { get; set; }
        public DbSet<Account> Accounts { get; set; }


        public TestReservationsDataContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public TestReservationsDataContext(DbContextOptions options) : base(options)
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