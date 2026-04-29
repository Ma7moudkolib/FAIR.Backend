using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FAIR.Application.DTOs.Profile
{
    public class UpdateCoachProfile
    {
        public required string Id { get; set; }

        [StringLength(100)]
        [NotNull]
        public required string FullName { get; set; }

        [StringLength(100)]
        [NotNull]
        public required string Specialization { get; set; }

        [Range(0, 70)]
        [NotNull]
        public int YearsOfExperience { get; set; }

        [StringLength(2000)]
        public string? Certifications { get; set; }

        [StringLength(100)]
        public string? CoachingLicenseLevel { get; set; }

        [StringLength(1000)]
        public string? PreferredTrainingMethodology { get; set; }

        [StringLength(200)]
        public string? TeamOrOrganization { get; set; }

        [Range(0, 10000)]
        public int AthletesCoachedCount { get; set; }

        [Range(0, 100)]
        public decimal CareerWinRate { get; set; }

        public bool IsAvailableForMentoring { get; set; }
    }
}
