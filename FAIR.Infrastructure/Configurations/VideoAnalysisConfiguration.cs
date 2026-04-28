using FAIR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAIR.Infrastructure.Configurations
{
    public class VideoAnalysisConfiguration : IEntityTypeConfiguration<VideoAnalysis>
    {
        public void Configure(EntityTypeBuilder<VideoAnalysis> builder)
        {
            builder.Property(x => x.Score).HasColumnType("decimal(6,2)");
            builder.Property(x => x.AiResultRaw).HasMaxLength(12000);
            builder.HasIndex(x => x.AthleteId);
            builder.HasIndex(x => x.CreatedDate);
        }
    }
}
