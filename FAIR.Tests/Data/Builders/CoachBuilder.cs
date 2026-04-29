using Bogus;
using FAIR.Domain.Entities.Identity;

namespace FAIR.Tests.Data.Builders
{
    /// <summary>
    /// Builder for creating Coach test entities with realistic data.
    /// </summary>
    public class CoachBuilder
    {
        private readonly Faker<Coach> _faker;
        private Coach _coach;

        public CoachBuilder()
        {
            _faker = new Faker<Coach>()
                .RuleFor(c => c.Id, _ => Guid.NewGuid().ToString())
                .RuleFor(c => c.UserName, f => f.Internet.UserName())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.FullName, f => f.Name.FullName())
                .RuleFor(c => c.Specialization, f => f.PickRandom(new[] { "Strength Training", "Technical Skills", "Tactical Analysis", "Mental Conditioning" }))
                .RuleFor(c => c.YearsOfExperience, f => (short)f.Random.Int(1, 30))
                .RuleFor(c => c.Certifications, f => f.Lorem.Sentences(f.Random.Int(1, 3)))
                .RuleFor(c => c.CoachingLicenseLevel, f => f.PickRandom(new[] { "Level 1", "Level 2", "Level 3", "Professional" }))
                .RuleFor(c => c.PreferredTrainingMethodology, f => f.Lorem.Sentence())
                .RuleFor(c => c.TeamOrOrganization, f => f.Company.CompanyName())
                .RuleFor(c => c.AthletesCoachedCount, f => f.Random.Int(0, 100))
                .RuleFor(c => c.CareerWinRate, f => f.Random.Decimal(0.30m, 0.95m))
                .RuleFor(c => c.IsAvailableForMentoring, f => f.Random.Bool());

            _coach = _faker.Generate();
        }

        public CoachBuilder WithEmail(string email)
        {
            _coach.Email = email;
            return this;
        }

        public CoachBuilder WithUsername(string username)
        {
            _coach.UserName = username;
            return this;
        }

        public CoachBuilder WithSpecialization(string specialization)
        {
            _coach.Specialization = specialization;
            return this;
        }

        public CoachBuilder WithYearsOfExperience(short years)
        {
            _coach.YearsOfExperience = years;
            return this;
        }

        public CoachBuilder WithCareerWinRate(decimal winRate)
        {
            _coach.CareerWinRate = Math.Clamp(winRate, 0, 1);
            return this;
        }

        public CoachBuilder WithAthletesCoachedCount(int count)
        {
            _coach.AthletesCoachedCount = count;
            return this;
        }

        public CoachBuilder WithAvailableForMentoring(bool available)
        {
            _coach.IsAvailableForMentoring = available;
            return this;
        }

        public Coach Build()
        {
            return _coach;
        }
    }
}
