using AutoMapper;
using FAIR.Application.DTOs.Profile;
using FAIR.Application.DTOs.Search;
using FAIR.Application.Exceptions;
using FAIR.Application.Mapping;
using FAIR.Application.Services.Implementations;
using FAIR.Domain.Entities.Identity;
using FAIR.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace FAIR.Tests.Unit.Services
{
    public class AthleteServiceTests
    {
        private readonly IMapper _mapper;

        public AthleteServiceTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingConfig>()).CreateMapper();
        }

        [Fact]
        public async Task SearchAsync_ShouldOrderByWeightedScoreDescending()
        {
            var repoManager = new Mock<IRepositoryManager>();
            var athleteRepo = new Mock<IAthleteRepository>();
            var videoRepo = new Mock<IVideoAnalysisRepository>();

            var athletes = new List<Athlete>
            {
                new() { Id = "a1", FullName = "A1", DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)), WinRate = 75m, RankingPoints = 1000m, MatchesPlayed = 20, AverageTrainingHoursPerWeek = 10, PrimarySport = "Tennis" },
                new() { Id = "a2", FullName = "A2", DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-22)), WinRate = 65m, RankingPoints = 800m, MatchesPlayed = 10, AverageTrainingHoursPerWeek = 5, PrimarySport = "Tennis" }
            }.AsQueryable();

            athleteRepo.Setup(r => r.QueryAthletes()).Returns(athletes);
            videoRepo.Setup(r => r.AverageScorePercentage("a1", It.IsAny<CancellationToken>())).ReturnsAsync(80m);
            videoRepo.Setup(r => r.AverageScorePercentage("a2", It.IsAny<CancellationToken>())).ReturnsAsync(50m);

            repoManager.SetupGet(r => r.AthleteRepository).Returns(athleteRepo.Object);
            repoManager.SetupGet(r => r.VideoAnalysis).Returns(videoRepo.Object);

            var service = new AthleteService(
                repoManager.Object,
                _mapper,
                GetPassValidator<UpdateAthleteProfile>(),
                GetPassValidator<ChangePasswordRequest>(),
                GetPassValidator<AthleteSearchFilter>());

            var result = await service.SearchAsync(new AthleteSearchFilter { PrimarySport = "Tennis" });

            result.Should().HaveCount(2);
            result[0].AthleteId.Should().Be("a1");
            result[0].WeightedScore.Should().BeGreaterThan(result[1].WeightedScore);
        }

        [Fact]
        public async Task SearchAsync_WhenValidationFails_ShouldThrowServiceValidationException()
        {
            var repoManager = new Mock<IRepositoryManager>();
            var failingValidator = new InlineValidator<AthleteSearchFilter>();
            failingValidator.RuleFor(x => x).Custom((_, context) => context.AddFailure("PrimarySport", "Primary sport is invalid"));

            var service = new AthleteService(
                repoManager.Object,
                _mapper,
                GetPassValidator<UpdateAthleteProfile>(),
                GetPassValidator<ChangePasswordRequest>(),
                failingValidator);

            var action = async () => await service.SearchAsync(new AthleteSearchFilter());

            await action.Should().ThrowAsync<ServiceValidationException>();
        }

        private static IValidator<T> GetPassValidator<T>() where T : class
        {
            var validator = new InlineValidator<T>();
            return validator;
        }
    }
}
