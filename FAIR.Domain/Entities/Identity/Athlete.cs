namespace FAIR.Domain.Entities.Identity
{
    public class Athlete : AppUser
    {
        public DateOnly DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? DominantHand { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public decimal BodyFatPercentage { get; set; }
        public decimal Wingspan { get; set; }
        public decimal Reach { get; set; }
        public string? PrimarySport { get; set; }
        public string? CurrentClub { get; set; }
        public int CareerStartYear { get; set; }
        public int YearsOfProfessionalExperience { get; set; }
        public int MatchesPlayed { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public decimal WinRate { get; set; }
        public decimal RankingPoints { get; set; }
        public decimal AverageTrainingHoursPerWeek { get; set; }
        public string? InjuryHistory { get; set; }
        public string? CareerHighlights { get; set; }
        public ICollection<VideoAnalysis>? VideoAnalyses { get; set; }
    }
}
