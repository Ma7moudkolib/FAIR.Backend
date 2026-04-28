using FAIR.Application.DTOs.Video;

namespace FAIR.Application.Services.Interfaces
{
    public interface IAiVideoService
    {
        Task<AiModelResponseDto> AnalyzeVideoAsync(Stream videoStream, string fileName, string? contentType, CancellationToken cancellationToken = default);
    }
}
