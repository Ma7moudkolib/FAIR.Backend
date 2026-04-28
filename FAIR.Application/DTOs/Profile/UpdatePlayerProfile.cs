using System.ComponentModel.DataAnnotations;

namespace FAIR.Application.DTOs.Profile
{
    public class UpdatePlayerProfile
    {
        public required string Id { get; set; }

        [StringLength(100)]
        public required string FullName { get; set; }

        public required DateOnly DateOfBirth { get; set; }

        [Range(0, 250)]
        public required decimal Weight { get; set; }

        [Range(0, 250)]
        public required decimal Height { get; set; }

        [StringLength(200)]
        public required string Address { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(30)]
        public string? DominantHand { get; set; }

        [Range(0, 100)]
        public decimal BodyFatPercentage { get; set; }

        [Range(0, 300)]
        public decimal Wingspan { get; set; }

        [Range(0, 300)]
        public decimal Reach { get; set; }

        [StringLength(100)]
        public string? PrimarySport { get; set; }

        [StringLength(150)]
        public string? CurrentClub { get; set; }

        [Range(1900, 2100)]
        public int CareerStartYear { get; set; }

        [Range(0, 60)]
        public int YearsOfProfessionalExperience { get; set; }

        [Range(0, 10000)]
        public int MatchesPlayed { get; set; }

        [Range(0, 10000)]
        public int MatchesWon { get; set; }

        [Range(0, 10000)]
        public int MatchesLost { get; set; }

        [Range(0, 100)]
        public decimal WinRate { get; set; }

        [Range(0, 1000000)]
        public decimal RankingPoints { get; set; }

        [Range(0, 168)]
        public decimal AverageTrainingHoursPerWeek { get; set; }

        [StringLength(2000)]
        public string? InjuryHistory { get; set; }

        [StringLength(2000)]
        public string? CareerHighlights { get; set; }
    }
}
