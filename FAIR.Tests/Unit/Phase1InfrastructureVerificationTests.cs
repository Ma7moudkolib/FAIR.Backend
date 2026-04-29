using FluentAssertions;
using FAIR.Tests.Common;
using FAIR.Tests.Data.Builders;
using FAIR.Tests.Fixtures;
using Xunit;

namespace FAIR.Tests.Unit
{
    /// <summary>
    /// Verification test to ensure Phase 1 infrastructure is working correctly.
    /// These tests validate the test setup, fixtures, and builders.
    /// </summary>
    public class Phase1InfrastructureVerificationTests : ServiceTestBase
    {
        public Phase1InfrastructureVerificationTests(MapperFixture mapperFixture)
            : base(mapperFixture)
        {
        }

        [Fact]
        public void Mapper_ShouldBeInitializedAndValid()
        {
            // Verify AutoMapper is initialized
            Mapper.Should().NotBeNull();
        }

        [Fact]
        public void MockRepositoryManager_ShouldHaveAllRepositories()
        {
            // Verify MockRepositoryManager exposes all required repositories
            MockRepositoryManager.Object.AthleteRepository.Should().NotBeNull();
            MockRepositoryManager.Object.UserRepository.Should().NotBeNull();
            MockRepositoryManager.Object.CoachRepository.Should().NotBeNull();
            MockRepositoryManager.Object.VideoAnalysis.Should().NotBeNull();
            MockRepositoryManager.Object.ChatRepository.Should().NotBeNull();
            MockRepositoryManager.Object.TokenManagement.Should().NotBeNull();
        }

        [Fact]
        public void MockServiceManager_ShouldHaveAllServices()
        {
            // Verify MockServiceManager exposes all required services
            MockServiceManager.Object.AuthenticationService.Should().NotBeNull();
            MockServiceManager.Object.AthleteService.Should().NotBeNull();
            MockServiceManager.Object.CoachService.Should().NotBeNull();
            MockServiceManager.Object.VideoService.Should().NotBeNull();
            MockServiceManager.Object.ChatService.Should().NotBeNull();
            MockServiceManager.Object.ConnectionMappingService.Should().NotBeNull();
        }

        [Fact]
        public void AthleteBuilder_ShouldCreateValidEntity()
        {
            // Verify AthleteBuilder generates valid entities
            var athlete = new AthleteBuilder().Build();

            athlete.Should().NotBeNull();
            athlete.Id.Should().NotBeNullOrEmpty();
            athlete.Email.Should().NotBeNullOrEmpty();
            athlete.UserName.Should().NotBeNullOrEmpty();
            athlete.FullName.Should().NotBeNullOrEmpty();
            athlete.WinRate.Should().BeGreaterThanOrEqualTo(0).And.BeLessThanOrEqualTo(1);
        }

        [Fact]
        public void CoachBuilder_ShouldCreateValidEntity()
        {
            // Verify CoachBuilder generates valid entities
            var coach = new CoachBuilder().Build();

            coach.Should().NotBeNull();
            coach.Id.Should().NotBeNullOrEmpty();
            coach.Email.Should().NotBeNullOrEmpty();
            coach.UserName.Should().NotBeNullOrEmpty();
            coach.FullName.Should().NotBeNullOrEmpty();
            coach.Specialization.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void MessageBuilder_ShouldCreateValidEntity()
        {
            // Verify MessageBuilder generates valid entities
            var message = new MessageBuilder().Build();

            message.Should().NotBeNull();
            message.Id.Should().NotBeNullOrEmpty();
            message.Content.Should().NotBeNullOrEmpty();
            message.SenderId.Should().NotBeNullOrEmpty();
            message.ReceiverId.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void VideoAnalysisBuilder_ShouldCreateValidEntity()
        {
            // Verify VideoAnalysisBuilder generates valid entities
            var analysis = new VideoAnalysisBuilder().Build();

            analysis.Should().NotBeNull();
            analysis.Id.Should().NotBe(Guid.Empty);
            analysis.AthleteId.Should().NotBeNullOrEmpty();
            analysis.Score.Should().BeGreaterThanOrEqualTo(0).And.BeLessThanOrEqualTo(100);
            analysis.ScorePercentage.Should().BeGreaterThanOrEqualTo(0).And.BeLessThanOrEqualTo(1);
        }

        [Fact]
        public void RefreshTokenBuilder_ShouldCreateValidEntity()
        {
            // Verify RefreshTokenBuilder generates valid entities
            var token = new RefreshTokenBuilder().Build();

            token.Should().NotBeNull();
            token.Id.Should().NotBeNullOrEmpty();
            token.Token.Should().NotBeNullOrEmpty();
            token.UserId.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void BuilderFluentApi_ShouldAllowCustomization()
        {
            // Verify builders support fluent customization
            var athlete = new AthleteBuilder()
                .WithEmail("custom@test.com")
                .WithWinRate(0.85m)
                .WithRankingPoints(2000m)
                .Build();

            athlete.Email.Should().Be("custom@test.com");
            athlete.WinRate.Should().Be(0.85m);
            athlete.RankingPoints.Should().Be(2000m);
        }
    }
}
