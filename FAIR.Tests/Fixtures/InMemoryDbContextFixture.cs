using FAIR.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FAIR.Tests.Fixtures
{
    /// <summary>
    /// Fixture that provides a fresh EF Core In-Memory database context for testing.
    /// Each test gets an isolated database instance to prevent cross-test contamination.
    /// </summary>
    public class InMemoryDbContextFixture : IAsyncLifetime
    {
        private readonly DbContextOptions<dbContext> _options;
        private dbContext? _context;

        public InMemoryDbContextFixture()
        {
            // Create unique database name for each fixture instance
            var databaseName = $"FairTestDb_{Guid.NewGuid()}";

            _options = new DbContextOptionsBuilder<dbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
        }

        public dbContext GetContext()
        {
            _context ??= new dbContext(_options);
            return _context;
        }

        /// <summary>
        /// Clears all data from the database while maintaining the context.
        /// </summary>
        public async Task ClearDatabaseAsync()
        {
            if (_context == null)
                return;

            // Delete all entities from all DbSets
            var players = _context.Athletes.ToList();
            _context.Athletes.RemoveRange(players);

            var coaches = _context.Coaches.ToList();
            _context.Coaches.RemoveRange(coaches);

            var messages = _context.Messages.ToList();
            _context.Messages.RemoveRange(messages);

            var videoAnalyses = _context.VideoAnalyses.ToList();
            _context.VideoAnalyses.RemoveRange(videoAnalyses);

            var refreshTokens = _context.RefreshToken.ToList();
            _context.RefreshToken.RemoveRange(refreshTokens);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Seeds test data into the database.
        /// </summary>
        public async Task SeedDatabaseAsync(Func<dbContext, Task> seeder)
        {
            var context = GetContext();
            await seeder(context);
            await context.SaveChangesAsync();
        }

        public async Task InitializeAsync()
        {
            // Initialize database schema (create in-memory context)
            var context = GetContext();
            await context.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            if (_context != null)
            {
                await _context.Database.EnsureDeletedAsync();
                await _context.DisposeAsync();
            }
        }
    }

    /// <summary>
    /// Collection fixture for sharing InMemoryDbContextFixture across test classes.
    /// </summary>
    [CollectionDefinition("InMemoryDb Collection")]
    public class InMemoryDbCollection : ICollectionFixture<InMemoryDbContextFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to define the collection that xUnit will use to apply the collection fixture.
    }
}
