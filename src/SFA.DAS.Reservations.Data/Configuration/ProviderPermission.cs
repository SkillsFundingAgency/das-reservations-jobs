using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Reservations.Data.Configuration
{
    public class ProviderPermission  : IEntityTypeConfiguration<Domain.Entities.ProviderPermission>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.ProviderPermission> builder)
        {
            builder.ToTable("ProviderPermission");
            builder.HasKey(x => new {x.AccountId, x.AccountLegalEntityId, x.ProviderId });

            builder.Property(x => x.AccountId).HasColumnName(@"AccountId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.AccountLegalEntityId).HasColumnName(@"AccountLegalEntityId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.ProviderId).HasColumnName(@"Ukprn").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.CanCreateCohort).HasColumnName(@"CanCreateCohort").HasColumnType("bit").IsRequired();
        }
    }
}
