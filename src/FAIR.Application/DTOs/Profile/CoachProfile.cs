namespace FAIR.Application.DTOs.Profile
{
    public class CoachProfile
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string? Certifications { get; set; }
        public string? CoachingLicenseLevel { get; set; }
        public string? PreferredTrainingMethodology { get; set; }
        public string? TeamOrOrganization { get; set; }
        public int AthletesCoachedCount { get; set; }
        public decimal CareerWinRate { get; set; }
        public bool IsAvailableForMentoring { get; set; }
    }
}
