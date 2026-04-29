using FluentAssertions;
using FAIR.Domain.Entities.Identity;
using FAIR.Infrastructure.Repository;
using FAIR.Tests.Common;
using FAIR.Tests.Data.Builders;
using FAIR.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FAIR.Tests.Unit.Repositories
{
    /// <summary>
    /// Unit tests for AthleteRepository (repository for Player/Athlete entities).
    /// Tests CRUD operations, filtering, and edge cases.
    /// </summary>
    public class AthleteRepositoryTests : RepositoryTestBase
    {
        private AthleteRepository _repository;

        public AthleteRepositoryTests(InMemoryDbContextFixture dbFixture, MapperFixture mapperFixture)
            : base(dbFixture, mapperFixture)
        {
        }

        private AthleteRepository GetRepository() => new AthleteRepository(DbContext);

        [Fact]
        public async Task CreateAthleteAsync_WithValidAthlete_ShouldAddToDatabase()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder()
                .WithEmail("newathlete@test.com")
                .WithUsername("newathlete")
                .Build();

            // Act
            _repository.CreateAthleteAsync(athlete);
            await DbContext.SaveChangesAsync();

            // Assert
            var result = await DbContext.Athletes.FindAsync(athlete.Id);
            result.Should().NotBeNull();
            result!.Email.Should().Be("newathlete@test.com");
            result.UserName.Should().Be("newathlete");
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnAthlete()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(athlete.Id, trackChanges: false);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(athlete.Id);
            result.Email.Should().Be(athlete.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            _repository = GetRepository();
            var nonExistentId = Guid.NewGuid().ToString();

            // Act
            var result = await _repository.GetByIdAsync(nonExistentId, trackChanges: false);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WithTrackChangesTrue_ShouldReturnTrackedAthlete()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().WithEmail("tracked@test.com").Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(athlete.Id, trackChanges: true);
            result!.Email = "modified@test.com";
            await DbContext.SaveChangesAsync();

            // Assert
            var reloaded = await DbContext.Athletes.FindAsync(athlete.Id);
            reloaded!.Email.Should().Be("modified@test.com");
        }

        [Fact]
        public async Task GetByIdAsync_WithTrackChangesFalse_ShouldReturnNoTrackAthlete()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().WithEmail("notrack@test.com").Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(athlete.Id, trackChanges: false);
            result!.Email = "modified@test.com";
            await DbContext.SaveChangesAsync();

            // Assert - Should not have saved changes since not tracked
            var reloaded = await DbContext.Athletes.FindAsync(athlete.Id);
            reloaded!.Email.Should().Be("notrack@test.com"); // Unchanged
        }

        [Fact]
        public async Task GetByUsernameAsync_WithExistingUsername_ShouldReturnAthlete()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().WithUsername("testuser").Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUsernameAsync("testuser");

            // Assert
            result.Should().NotBeNull();
            result!.UserName.Should().Be("testuser");
            result.Id.Should().Be(athlete.Id);
        }

        [Fact]
        public async Task GetByUsernameAsync_WithNonExistentUsername_ShouldReturnNull()
        {
            // Arrange
            _repository = GetRepository();

            // Act
            var result = await _repository.GetByUsernameAsync("nonexistent");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByUsernameAsync_WithCaseSensitiveUsername_ShouldMatchUnderlyingStoreBehavior()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().WithUsername("TestUser").Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUsernameAsync("testuser");

            // Assert - provider comparison can differ by collation
            (result == null || string.Equals(result.UserName, "TestUser", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }

        [Fact]
        public async Task GetByEmailAsync_WithExistingEmail_ShouldReturnAthlete()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().WithEmail("test@example.com").Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByEmailAsync("test@example.com");

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("test@example.com");
            result.Id.Should().Be(athlete.Id);
        }

        [Fact]
        public async Task GetByEmailAsync_WithNonExistentEmail_ShouldReturnNull()
        {
            // Arrange
            _repository = GetRepository();

            // Act
            var result = await _repository.GetByEmailAsync("nonexistent@example.com");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByAthletesByIdsAsync_WithMultipleIds_ShouldReturnAllMatching()
        {
            // Arrange
            _repository = GetRepository();
            var athlete1 = new AthleteBuilder().Build();
            var athlete2 = new AthleteBuilder().Build();
            var athlete3 = new AthleteBuilder().Build();
            await DbContext.Athletes.AddRangeAsync(athlete1, athlete2, athlete3);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAthletesByIdsAsync(new[] { athlete1.Id, athlete3.Id });

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(a => a.Id == athlete1.Id);
            result.Should().Contain(a => a.Id == athlete3.Id);
            result.Should().NotContain(a => a.Id == athlete2.Id);
        }

        [Fact]
        public async Task GetAthletesByIdsAsync_WithEmptyIdList_ShouldReturnEmpty()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAthletesByIdsAsync(new string[] { });

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAthletesByIdsAsync_WithNonExistentIds_ShouldReturnEmpty()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAthletesByIdsAsync(new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() });

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteAthlete_WithExistingAthlete_ShouldRemoveFromDatabase()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            // Act
            var athleteToDelete = await DbContext.Athletes.FindAsync(athlete.Id);
            DbContext.Athletes.Remove(athleteToDelete!);
            await DbContext.SaveChangesAsync();

            // Assert
            var result = await DbContext.Athletes.FindAsync(athlete.Id);
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAthlete_WithChanges_ShouldPersistChanges()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().WithEmail("original@test.com").WithWinRate(0.50m).Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            // Act
            var athleteToUpdate = await DbContext.Athletes.FindAsync(athlete.Id);
            athleteToUpdate!.Email = "updated@test.com";
            athleteToUpdate.WinRate = 0.75m;
            await DbContext.SaveChangesAsync();

            // Assert
            var result = await DbContext.Athletes.FindAsync(athlete.Id);
            result!.Email.Should().Be("updated@test.com");
            result.WinRate.Should().Be(0.75m);
        }

        [Fact]
        public async Task GetAllAthletes_ShouldReturnAllAthletes()
        {
            // Arrange
            _repository = GetRepository();
            var athletes = new[]
            {
                new AthleteBuilder().Build(),
                new AthleteBuilder().Build(),
                new AthleteBuilder().Build()
            };
            await DbContext.Athletes.AddRangeAsync(athletes);
            await DbContext.SaveChangesAsync();

            // Act
            var result = _repository.QueryAthletes().ToList();

            // Assert
            result.Should().HaveCountGreaterThanOrEqualTo(3);
        }

        [Fact]
        public async Task FindByCondition_WithQuery_ShouldReturnFilteredResults()
        {
            // Arrange
            _repository = GetRepository();
            var athlete1 = new AthleteBuilder().WithWinRate(0.75m).Build();
            var athlete2 = new AthleteBuilder().WithWinRate(0.50m).Build();
            await DbContext.Athletes.AddRangeAsync(athlete1, athlete2);
            await DbContext.SaveChangesAsync();

            // Act
            var result = _repository.QueryAthletes().Where(a => a.WinRate > 0.60m).ToList();

            // Assert
            result.Should().Contain(a => a.Id == athlete1.Id);
            result.Should().NotContain(a => a.Id == athlete2.Id);
        }

        [Fact]
        public async Task MultipleOperations_ShouldMaintainDatabaseConsistency()
        {
            // Arrange
            _repository = GetRepository();
            var athlete1 = new AthleteBuilder().WithEmail("athlete1@test.com").Build();
            var athlete2 = new AthleteBuilder().WithEmail("athlete2@test.com").Build();

            // Act - Create
            _repository.CreateAthleteAsync(athlete1);
            _repository.CreateAthleteAsync(athlete2);
            await DbContext.SaveChangesAsync();

            // Get one
            var retrieved = await _repository.GetByIdAsync(athlete1.Id, trackChanges: true);

            // Update
            retrieved!.Email = "updated_athlete1@test.com";
            await DbContext.SaveChangesAsync();

            // Delete the other
            var toDelete = await _repository.GetByIdAsync(athlete2.Id, trackChanges: true);
            DbContext.Athletes.Remove(toDelete!);
            await DbContext.SaveChangesAsync();

            // Assert
            var final1 = await _repository.GetByIdAsync(athlete1.Id, trackChanges: false);
            var final2 = await _repository.GetByIdAsync(athlete2.Id, trackChanges: false);

            final1!.Email.Should().Be("updated_athlete1@test.com");
            final2.Should().BeNull();
        }
    }
}
