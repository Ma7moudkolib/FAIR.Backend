using FAIR.Application.DTOs.Video;

namespace FAIR.Application.Services.Interfaces
{
    public interface IVideoService
    {
        Task<VideoAnalysisResponseDto> AnalyzeAsync(VideoUploadDto videoUploadDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<VideoAnalysisResponseDto>> GetAthleteAnalysisHistoryAsync(Guid athleteId, CancellationToken cancellationToken = default);
        Task<VideoAnalysisResponseDto?> GetAnalysisDetailsAsync(Guid analysisId, CancellationToken cancellationToken = default);
    }
}
