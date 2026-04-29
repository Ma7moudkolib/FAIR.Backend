using FAIR.Domain.Entities;

namespace FAIR.Domain.Interfaces
{
    public interface IVideoAnalysisRepository
    {
        void CreateVideoAnalysis(VideoAnalysis videoAnalysis);
        Task<IEnumerable<VideoAnalysis>> GetAllByAthleteIdAsync(string athleteId, bool trackChanges);
        Task<VideoAnalysis> GetByIdAsync(Guid analysisId, bool trackChanges);
        Task<decimal> AverageScorePercentage(string athleteId, CancellationToken cancellationToken = default);
    }
}
