using FluentAssertions;
using FAIR.Domain.Entities;
using FAIR.Domain.Enums;
using FAIR.Infrastructure.Repository;
using FAIR.Tests.Common;
using FAIR.Tests.Data.Builders;
using FAIR.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FAIR.Tests.Unit.Repositories
{
    /// <summary>
    /// Unit tests for VideoAnalysisRepository.
    /// Tests video analysis CRUD operations, filtering by athlete, and metrics calculations.
    /// </summary>
    public class VideoAnalysisRepositoryTests : RepositoryTestBase
    {
        private VideoAnalysisRepository _repository;

        public VideoAnalysisRepositoryTests(InMemoryDbContextFixture dbFixture, MapperFixture mapperFixture)
            : base(dbFixture, mapperFixture)
        {
        }

        private VideoAnalysisRepository GetRepository() => new VideoAnalysisRepository(DbContext);

        [Fact]
        public async Task CreateVideoAnalysis_WithValidAnalysis_ShouldPersist()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            var analysis = new VideoAnalysisBuilder()
                .WithAthleteId(athlete.Id)
                .WithScore(85.5m)
                .Build();

            // Act
            _repository.CreateVideoAnalysis(analysis);
            await DbContext.SaveChangesAsync();

            // Assert
            var result = await DbContext.VideoAnalyses.FindAsync(analysis.Id);
            result.Should().NotBeNull();
            result!.AthleteId.Should().Be(athlete.Id);
            result.Score.Should().Be(85.5m);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingAnalysis_ShouldReturn()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            var analysis = new VideoAnalysisBuilder().WithAthleteId(athlete.Id).Build();
            await DbContext.VideoAnalyses.AddAsync(analysis);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(analysis.Id, trackChanges: false);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(analysis.Id);
            result.AthleteId.Should().Be(athlete.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            _repository = GetRepository();

            // Act
            var result = await _repository.GetByIdAsync(Guid.NewGuid(), trackChanges: false);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllByAthleteIdAsync_ShouldReturnAllAnalysesForAthlete()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().Build();
            var otherAthlete = new AthleteBuilder().Build();
            await DbContext.Athletes.AddRangeAsync(athlete, otherAthlete);
            await DbContext.SaveChangesAsync();

            var analysis1 = new VideoAnalysisBuilder().WithAthleteId(athlete.Id).Build();
            var analysis2 = new VideoAnalysisBuilder().WithAthleteId(athlete.Id).Build();
            var analysis3 = new VideoAnalysisBuilder().WithAthleteId(otherAthlete.Id).Build();
            await DbContext.VideoAnalyses.AddRangeAsync(analysis1, analysis2, analysis3);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllByAthleteIdAsync(athlete.Id, trackChanges: false);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(a => a.AthleteId.Should().Be(athlete.Id));
        }

        [Fact]
        public async Task GetAllByAthleteIdAsync_WithNoAnalyses_ShouldReturnEmpty()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllByAthleteIdAsync(athlete.Id, trackChanges: false);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllByAthleteIdAsync_WithTrackChanges_ShouldReturnTrackedEntities()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            var analysis = new VideoAnalysisBuilder().WithAthleteId(athlete.Id).WithScore(75m).Build();
            await DbContext.VideoAnalyses.AddAsync(analysis);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllByAthleteIdAsync(athlete.Id, trackChanges: true);
            var entity = result.FirstOrDefault();
            entity!.Score = 90m;
            await DbContext.SaveChangesAsync();

            // Assert
            var reloaded = await DbContext.VideoAnalyses.FindAsync(analysis.Id);
            reloaded!.Score.Should().Be(90m);
        }

        [Fact]
        public async Task AverageScorePercentage_ShouldCalculateCorrectly()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            var analysis1 = new VideoAnalysisBuilder().WithAthleteId(athlete.Id).WithScore(80m).Build();
            var analysis2 = new VideoAnalysisBuilder().WithAthleteId(athlete.Id).WithScore(60m).Build();
            var analysis3 = new VideoAnalysisBuilder().WithAthleteId(athlete.Id).WithScore(100m).Build();
            await DbContext.VideoAnalyses.AddRangeAsync(analysis1, analysis2, analysis3);
            await DbContext.SaveChangesAsync();

            // Act
            var average = await _repository.AverageScorePercentage(athlete.Id);

            // Assert
            // Scores are 0.80, 0.60, 1.00 → average = 0.8
            average.Should().BeApproximately(0.8m, 0.01m);
        }

        [Fact]
        public async Task AverageScorePercentage_WithNoAnalyses_ShouldReturnZero()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            // Act
            var average = await _repository.AverageScorePercentage(athlete.Id);

            // Assert
            average.Should().Be(0m);
        }

        [Fact]
        public async Task AverageScorePercentage_ShouldExcludeOtherAthletes()
        {
            // Arrange
            _repository = GetRepository();
            var athlete1 = new AthleteBuilder().Build();
            var athlete2 = new AthleteBuilder().Build();
            await DbContext.Athletes.AddRangeAsync(athlete1, athlete2);
            await DbContext.SaveChangesAsync();

            var analysis1 = new VideoAnalysisBuilder().WithAthleteId(athlete1.Id).WithScore(100m).Build();
            var analysis2 = new VideoAnalysisBuilder().WithAthleteId(athlete2.Id).WithScore(50m).Build();
            await DbContext.VideoAnalyses.AddRangeAsync(analysis1, analysis2);
            await DbContext.SaveChangesAsync();

            // Act
            var average1 = await _repository.AverageScorePercentage(athlete1.Id);
            var average2 = await _repository.AverageScorePercentage(athlete2.Id);

            // Assert
            average1.Should().Be(1.0m);
            average2.Should().Be(0.5m);
        }

        [Fact]
        public async Task DeleteAnalysis_ShouldRemoveFromDatabase()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            var analysis = new VideoAnalysisBuilder().WithAthleteId(athlete.Id).Build();
            await DbContext.VideoAnalyses.AddAsync(analysis);
            await DbContext.SaveChangesAsync();

            // Act
            var toDelete = await DbContext.VideoAnalyses.FindAsync(analysis.Id);
            _repository.Delete(toDelete!);
            await DbContext.SaveChangesAsync();

            // Assert
            var result = await DbContext.VideoAnalyses.FindAsync(analysis.Id);
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAnalysis_ShouldPersistChanges()
        {
            // Arrange
            _repository = GetRepository();
            var athlete = new AthleteBuilder().Build();
            await DbContext.Athletes.AddAsync(athlete);
            await DbContext.SaveChangesAsync();

            var analysis = new VideoAnalysisBuilder()
                .WithAthleteId(athlete.Id)
                .WithScore(50m)
                .WithProcessingStatus(AnalysisProcessingStatus.Pending)
                .Build();
            await DbContext.VideoAnalyses.AddAsync(analysis);
            await DbContext.SaveChangesAsync();

            // Act
            var toUpdate = await DbContext.VideoAnalyses.FindAsync(analysis.Id);
            toUpdate!.Score = 85m;
            toUpdate.ProcessingStatus = AnalysisProcessingStatus.Completed;
            _repository.Update(toUpdate);
            await DbContext.SaveChangesAsync();

            // Assert
            var result = await DbContext.VideoAnalyses.FindAsync(analysis.Id);
            result!.Score.Should().Be(85m);
            result.ProcessingStatus.Should().Be(AnalysisProcessingStatus.Completed);
        }
    }
}
