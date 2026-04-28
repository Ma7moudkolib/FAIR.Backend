using FAIR.Domain.Entities;
using FAIR.Domain.Entities.Chat;
using FAIR.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FAIR.Infrastructure.Context
{
    public class dbContext(DbContextOptions<DbContext> options) : IdentityDbContext<AppUser>(options)
    {

        public DbSet<Player> Players { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<VideoAnalysis> VideoAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(dbContext).Assembly);
        }
    }
}
