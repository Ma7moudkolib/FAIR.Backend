using FAIR.Domain.Entities;
using FAIR.Domain.Interfaces;
using FAIR.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FAIR.Infrastructure.Repository
{
    public class VideoAnalysisRepository(dbContext context) : RepositoryBase<VideoAnalysis>(context), IVideoAnalysisRepository
    {
        public void CreateVideoAnalysis(VideoAnalysis videoAnalysis)=> Create(videoAnalysis);

        public async Task<IEnumerable<VideoAnalysis>> GetAllByAthleteIdAsync(string athleteId, bool trackChanges) => 
            await FindByCondition(x => x.AthleteId == athleteId, trackChanges).ToListAsync();

        public async Task<VideoAnalysis> GetByIdAsync(Guid analysisId, bool trackChanges) 
        => await FindByCondition(x => x.Id == analysisId, trackChanges).FirstOrDefaultAsync();

        public async Task<decimal> AverageScorePercentage(string athleteId, CancellationToken cancellationToken = default)
        {
            var query = context.VideoAnalyses.Where(x => x.AthleteId == athleteId);
            if (!await query.AnyAsync(cancellationToken)) return 0;
            return await query.AverageAsync(x => x.ScorePercentage, cancellationToken);
        }
    }
}
