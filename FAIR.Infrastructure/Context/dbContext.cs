using FAIR.Domain.Entities;
using FAIR.Domain.Entities.Chat;
using FAIR.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FAIR.Infrastructure.Context
{
    public class dbContext : DbContext
    {
        public dbContext(DbContextOptions<dbContext> options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Athlete> Athletes { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<VideoAnalysis> VideoAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Athlete>().ToTable("Athletes");
            builder.Entity<Coach>().ToTable("Coaches");

            builder.ApplyConfigurationsFromAssembly(typeof(dbContext).Assembly);
        }
    }
}
