using AutoMapper;
using FAIR.Application.DTOs.Search;
using FAIR.Application.Services.Interfaces;
using FAIR.Domain.Interfaces;

namespace FAIR.Application.Services.Implementations
{
    public class AthleteSearchService(IRepositoryManager repositoryManager, IMapper mapper) : IAthleteSearchService
    {
        public async Task<IReadOnlyList<AthleteSearchResult>> SearchAsync(AthleteSearchFilter filter, CancellationToken cancellationToken = default)
        {
            var query = repositoryManager.AthleteSearchRepository.QueryAthletes();

            if (!string.IsNullOrWhiteSpace(filter.PrimarySport))
            {
                var sport = filter.PrimarySport.Trim();
                query = query.Where(a => a.PrimarySport != null && a.PrimarySport == sport);
            }

            if (!string.IsNullOrWhiteSpace(filter.Location))
            {
                var location = filter.Location.Trim();
                query = query.Where(a =>
                    (a.Country != null && a.Country.Contains(location)) ||
                    (a.City != null && a.City.Contains(location)) ||
                    (a.Address != null && a.Address.Contains(location)));
            }

            if (filter.MinWinRate.HasValue)
            {
                query = query.Where(a => a.WinRate >= filter.MinWinRate.Value);
            }

            if (filter.MaxWinRate.HasValue)
            {
                query = query.Where(a => a.WinRate <= filter.MaxWinRate.Value);
            }

            if (filter.MinRankingPoints.HasValue)
            {
                query = query.Where(a => a.RankingPoints >= filter.MinRankingPoints.Value);
            }

            if (filter.MaxRankingPoints.HasValue)
            {
                query = query.Where(a => a.RankingPoints <= filter.MaxRankingPoints.Value);
            }

            var athletes = query.ToList();
            var results = mapper.Map<List<AthleteSearchResult>>(athletes);

            foreach (var result in results)
            {
                var athlete = athletes.First(a => a.Id == result.AthleteId);
                result.Age = CalculateAge(athlete.DateOfBirth);
                result.AverageScorePercentage = await repositoryManager.VideoAnalysis.AverageScorePercentage(result.AthleteId);
                result.WeightedScore = CalculateWeightedScore(athlete, result.AverageScorePercentage);
            }

            var filtered = results.AsQueryable();

            if (filter.MinAge.HasValue)
            {
                filtered = filtered.Where(r => r.Age >= filter.MinAge.Value);
            }

            if (filter.MaxAge.HasValue)
            {
                filtered = filtered.Where(r => r.Age <= filter.MaxAge.Value);
            }

            if (filter.MinSkillScore.HasValue)
            {
                filtered = filtered.Where(r => r.WeightedScore >= filter.MinSkillScore.Value);
            }

            return filtered
                .OrderByDescending(r => r.WeightedScore)
                .ThenByDescending(r => r.WinRate)
                .ToList();
        }

        private static int CalculateAge(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        private static decimal CalculateWeightedScore(Domain.Entities.Identity.Player athlete, decimal averageScore)
        {
            const decimal winRateWeight = 0.30m;
            const decimal rankingWeight = 0.25m;
            const decimal aiPerformanceWeight = 0.25m;
            const decimal matchesWeight = 0.10m;
            const decimal trainingWeight = 0.10m;

            var normalizedRanking = Math.Min(athlete.RankingPoints / 1000m, 100m);
            var normalizedMatches = Math.Min(athlete.MatchesPlayed / 10m, 100m);
            var normalizedTraining = Math.Min(athlete.AverageTrainingHoursPerWeek * 2m, 100m);

            var weighted = (athlete.WinRate * winRateWeight)
                + (normalizedRanking * rankingWeight)
                + (averageScore * aiPerformanceWeight)
                + (normalizedMatches * matchesWeight)
                + (normalizedTraining * trainingWeight);

            return Math.Round(weighted, 2);
        }
    }
}
