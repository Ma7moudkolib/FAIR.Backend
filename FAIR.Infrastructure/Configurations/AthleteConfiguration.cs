using FAIR.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAIR.Infrastructure.Configurations
{
    public class AthleteConfiguration : IEntityTypeConfiguration<Athlete>
    {
        public void Configure(EntityTypeBuilder<Athlete> builder)
        {
            builder.Property(x => x.FullName).HasMaxLength(150);
            builder.Property(x => x.Address).HasMaxLength(250);
            builder.Property(x => x.Country).HasMaxLength(100);
            builder.Property(x => x.City).HasMaxLength(100);
            builder.Property(x => x.DominantHand).HasMaxLength(30);
            builder.Property(x => x.PrimarySport).HasMaxLength(100);
            builder.Property(x => x.CurrentClub).HasMaxLength(150);
            builder.Property(x => x.Weight).HasColumnType("decimal(6,2)");
            builder.Property(x => x.Height).HasColumnType("decimal(6,2)");
            builder.Property(x => x.BodyFatPercentage).HasColumnType("decimal(5,2)");
            builder.Property(x => x.Wingspan).HasColumnType("decimal(6,2)");
            builder.Property(x => x.Reach).HasColumnType("decimal(6,2)");
            builder.Property(x => x.WinRate).HasColumnType("decimal(5,2)");
            builder.Property(x => x.RankingPoints).HasColumnType("decimal(12,2)");
            builder.Property(x => x.AverageTrainingHoursPerWeek).HasColumnType("decimal(6,2)");
            builder.Property(x => x.InjuryHistory).HasMaxLength(2000);
            builder.Property(x => x.CareerHighlights).HasMaxLength(2000);
        }
    }
}
