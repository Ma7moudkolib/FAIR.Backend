using FAIR.Domain.Entities.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAIR.Infrastructure.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.Property(x => x.Content).HasMaxLength(2000);
            builder.Property(x => x.SenderName).HasMaxLength(150);
            builder.HasIndex(x => new { x.SenderId, x.ReceiverId, x.CreateData });
        }
    }
}
