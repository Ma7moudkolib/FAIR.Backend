using FAIR.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAIR.Infrastructure.Configurations
{
    public class CoachConfiguration : IEntityTypeConfiguration<Coach>
    {
        public void Configure(EntityTypeBuilder<Coach> builder)
        {
            builder.Property(x => x.FullName).HasMaxLength(150);
            builder.Property(x => x.Specialization).HasMaxLength(100);
            builder.Property(x => x.Certifications).HasMaxLength(2000);
            builder.Property(x => x.CoachingLicenseLevel).HasMaxLength(100);
            builder.Property(x => x.PreferredTrainingMethodology).HasMaxLength(1000);
            builder.Property(x => x.TeamOrOrganization).HasMaxLength(200);
            builder.Property(x => x.CareerWinRate).HasColumnType("decimal(5,2)");
        }
    }
}
