namespace FAIR.Application.DTOs.Search
{
    public class AthleteSearchResult
    {
        public string AthleteId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? PrimarySport { get; set; }
        public int Age { get; set; }
        public decimal WinRate { get; set; }
        public decimal RankingPoints { get; set; }
        public decimal AverageScorePercentage { get; set; }
        public decimal WeightedScore { get; set; }
    }
}
