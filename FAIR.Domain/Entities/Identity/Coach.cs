namespace FAIR.Domain.Entities.Identity
{
    public class Coach : AppUser
    {
        public string? Specialization { get; set; }
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
