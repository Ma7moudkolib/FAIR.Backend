using AutoMapper;
using FAIR.Application.DTOs.Video;
using FAIR.Application.Exceptions;
using FAIR.Application.Services.Interfaces;
using FAIR.Domain.Entities;
using FAIR.Domain.Interfaces;
using FluentValidation;

namespace FAIR.Application.Services.Implementations
{
    public class VideoService(
        IRepositoryManager repositoryManager,
        IAiVideoService aiVideoService,
        IMapper mapper,
        IValidator<VideoUploadDto> videoUploadDtoValidator) : IVideoService
    {
        public async Task<VideoAnalysisResponseDto> AnalyzeAsync(VideoUploadDto videoUploadDto, CancellationToken cancellationToken = default)
        {
            var validationResult = await videoUploadDtoValidator.ValidateAsync(videoUploadDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ServiceValidationException(validationResult.Errors);
            }

            await using var videoStream = videoUploadDto.Video.OpenReadStream();
            var aiResponse = await aiVideoService.AnalyzeVideoAsync(
                videoStream,
                videoUploadDto.Video.FileName,
                videoUploadDto.Video.ContentType,
                cancellationToken);

            var videoAnalysis = mapper.Map<VideoAnalysis>(aiResponse, opts =>
            {
                opts.Items["AthleteId"] = videoUploadDto.AthleteId;
                opts.Items["CreatedDate"] = DateTime.UtcNow;
            });

            repositoryManager.VideoAnalysis.CreateVideoAnalysis(videoAnalysis);
            await repositoryManager.SaveAsync(cancellationToken);
            return mapper.Map<VideoAnalysisResponseDto>(videoAnalysis);
        }

        public async Task<IEnumerable<VideoAnalysisResponseDto>> GetAthleteAnalysisHistoryAsync(Guid athleteId, CancellationToken cancellationToken = default)
        {
            var athleteIdStr = athleteId.ToString();
            var analyses = await repositoryManager.VideoAnalysis.GetAllByAthleteIdAsync(athleteIdStr, trackChanges: false);
            return mapper.Map<IEnumerable<VideoAnalysisResponseDto>>(analyses);
        }

        public async Task<VideoAnalysisResponseDto?> GetAnalysisDetailsAsync(Guid analysisId, CancellationToken cancellationToken = default)
        {
            var analysis = await repositoryManager.VideoAnalysis.GetByIdAsync(analysisId, trackChanges: false);
            return analysis == null ? null : mapper.Map<VideoAnalysisResponseDto>(analysis);
        }
    }
}
