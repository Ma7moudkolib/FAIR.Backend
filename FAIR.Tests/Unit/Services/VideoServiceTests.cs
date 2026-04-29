using AutoMapper;
using FAIR.Application.DTOs.Video;
using FAIR.Application.Exceptions;
using FAIR.Application.Mapping;
using FAIR.Application.Services.Implementations;
using FAIR.Application.Services.Interfaces;
using FAIR.Domain.Entities;
using FAIR.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace FAIR.Tests.Unit.Services
{
    public class VideoServiceTests
    {
        private readonly IMapper _mapper;

        public VideoServiceTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingConfig>()).CreateMapper();
        }

        [Fact]
        public async Task AnalyzeAsync_WhenAiSucceeds_ShouldMapAndPersistAnalysis()
        {
            var repositoryManager = new Mock<IRepositoryManager>();
            var videoRepo = new Mock<IVideoAnalysisRepository>();
            var aiVideoService = new Mock<IAiVideoService>();

            VideoAnalysis? captured = null;
            videoRepo.Setup(v => v.CreateVideoAnalysis(It.IsAny<VideoAnalysis>()))
                .Callback<VideoAnalysis>(va => captured = va);

            repositoryManager.SetupGet(r => r.VideoAnalysis).Returns(videoRepo.Object);
            repositoryManager.Setup(r => r.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            aiVideoService.Setup(a => a.AnalyzeVideoAsync(It.IsAny<Stream>(), "video.mp4", "video/mp4", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AiModelResponseDto
                {
                    Score = 88,
                    RawJson = "{\"ok\":true}",
                    Metrics = new AiModelMetricsDto { OverallScore = 88 }
                });

            var file = new FormFile(new MemoryStream(new byte[] { 1, 2, 3 }), 0, 3, "video", "video.mp4")
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/mp4"
            };

            var service = new VideoService(repositoryManager.Object, aiVideoService.Object, _mapper, GetPassValidator<VideoUploadDto>());

            var dto = new VideoUploadDto { AthleteId = Guid.NewGuid(), Video = file };
            var response = await service.AnalyzeAsync(dto);

            captured.Should().NotBeNull();
            captured!.Score.Should().Be(88);
            repositoryManager.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            response.Score.Should().Be(88);
        }

        [Fact]
        public async Task AnalyzeAsync_WhenValidationFails_ShouldThrowServiceValidationException()
        {
            var repositoryManager = new Mock<IRepositoryManager>();
            var aiVideoService = new Mock<IAiVideoService>();
            var validator = new InlineValidator<VideoUploadDto>();
            validator.RuleFor(x => x).Custom((_, context) => context.AddFailure("Video", "Video is required"));
            var service = new VideoService(repositoryManager.Object, aiVideoService.Object, _mapper, validator);

            var file = new FormFile(new MemoryStream(new byte[] { 1 }), 0, 1, "video", "video.mp4")
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/mp4"
            };

            var action = async () => await service.AnalyzeAsync(new VideoUploadDto { AthleteId = Guid.NewGuid(), Video = file });
            await action.Should().ThrowAsync<ServiceValidationException>();
        }

        private static IValidator<T> GetPassValidator<T>() where T : class => new InlineValidator<T>();
    }
}
