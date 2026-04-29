using FAIR.Infrastructure.Context;
using FAIR.Tests.Fixtures;
using FAIR.Tests.Mocks;
using Xunit;

namespace FAIR.Tests.Common
{
    /// <summary>
    /// Base class for repository tests.
    /// Provides common setup with in-memory database and mock managers.
    /// </summary>
    public abstract class RepositoryTestBase : IClassFixture<InMemoryDbContextFixture>, IClassFixture<MapperFixture>
    {
        protected readonly InMemoryDbContextFixture DbFixture;
        protected readonly MapperFixture MapperFixture;
        protected readonly MockRepositoryManager MockRepositoryManager;

        protected dbContext DbContext => DbFixture.GetContext();

        protected RepositoryTestBase(InMemoryDbContextFixture dbFixture, MapperFixture mapperFixture)
        {
            DbFixture = dbFixture;
            MapperFixture = mapperFixture;
            MockRepositoryManager = new MockRepositoryManager();
        }

        /// <summary>
        /// Seeds test data into the database.
        /// </summary>
        protected async Task SeedDatabaseAsync(Func<dbContext, Task> seeder)
        {
            await DbFixture.SeedDatabaseAsync(seeder);
        }

        /// <summary>
        /// Clears all data from the database.
        /// </summary>
        protected async Task ClearDatabaseAsync()
        {
            await DbFixture.ClearDatabaseAsync();
        }
    }
}
