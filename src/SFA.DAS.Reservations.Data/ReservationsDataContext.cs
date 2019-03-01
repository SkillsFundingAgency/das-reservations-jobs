using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data
{
    public interface IReservationsDataContext
    {
        DbSet<Course> Apprenticeships { get; set; }
        int SaveChanges();
    }
    public class ReservationsDataContext :DbContext, IReservationsDataContext
    {
        public DbSet<Course> Apprenticeships { get; set; }
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

            base.OnModelCreating(modelBuilder);
        }
    }
}