namespace FAIR.Application.DTOs.Search
{
    public class AthleteSearchFilter
    {
        public string? Location { get; set; }
        public string? PrimarySport { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public decimal? MinWinRate { get; set; }
        public decimal? MaxWinRate { get; set; }
        public decimal? MinRankingPoints { get; set; }
        public decimal? MaxRankingPoints { get; set; }
        public decimal? MinSkillScore { get; set; }
        public bool IncludeInactive { get; set; }
    }
}
