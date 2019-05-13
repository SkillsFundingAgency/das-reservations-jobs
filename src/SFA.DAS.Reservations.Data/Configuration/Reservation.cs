using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Reservations.Data.Configuration
{
    public class Reservation : IEntityTypeConfiguration<Domain.Entities.Reservation>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Reservation> builder)
        {
            builder.ToTable("Reservation");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.Status).HasColumnName(@"Status").HasColumnType("tinyint").IsRequired();
        }
    }
}
