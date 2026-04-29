using Bogus;
using FAIR.Domain.Entities.Identity;

namespace FAIR.Tests.Data.Builders
{
    /// <summary>
    /// Builder for creating Athlete test entities with realistic data.
    /// Uses Bogus to generate consistent, realistic test data.
    /// </summary>
    public class AthleteBuilder
    {
        private readonly Faker<Athlete> _faker;
        private Athlete _athlete;

        public AthleteBuilder()
        {
            _faker = new Faker<Athlete>()
                .RuleFor(p => p.Id, _ => Guid.NewGuid().ToString())
                .RuleFor(p => p.UserName, f => f.Internet.UserName())
                .RuleFor(p => p.Email, f => f.Internet.Email())
                .RuleFor(p => p.FullName, f => f.Name.FullName())
                .RuleFor(p => p.DateOfBirth, f => DateOnly.FromDateTime(f.Date.Past(30, DateTime.Now.AddYears(-15))))
                .RuleFor(p => p.Address, f => f.Address.FullAddress())
                .RuleFor(p => p.Country, f => f.Address.Country())
                .RuleFor(p => p.City, f => f.Address.City())
                .RuleFor(p => p.DominantHand, f => f.PickRandom(new[] { "Right", "Left", "Ambidextrous" }))
                .RuleFor(p => p.Weight, f => f.Random.Decimal(50, 120))
                .RuleFor(p => p.Height, f => f.Random.Decimal(160, 210))
                .RuleFor(p => p.Wingspan, f => f.Random.Decimal(160, 220))
                .RuleFor(p => p.Reach, f => f.Random.Decimal(50, 85))
                .RuleFor(p => p.BodyFatPercentage, f => f.Random.Decimal(5, 25))
                .RuleFor(p => p.WinRate, f => f.Random.Decimal(0.20m, 0.95m))
                .RuleFor(p => p.RankingPoints, f => f.Random.Decimal(0, 5000))
                .RuleFor(p => p.PrimarySport, f => f.PickRandom(new[] { "Tennis", "Basketball", "Football", "Badminton", "Volleyball" }))
                .RuleFor(p => p.CurrentClub, f => f.Company.CompanyName())
                .RuleFor(p => p.CareerStartYear, f => (short)(DateTime.Now.Year - f.Random.Int(2, 15)))
                .RuleFor(p => p.YearsOfProfessionalExperience, f => (short)f.Random.Int(0, 15))
                .RuleFor(p => p.MatchesPlayed, f => f.Random.Int(0, 500))
                .RuleFor(p => p.MatchesWon, _ => 0)
                .RuleFor(p => p.MatchesLost, _ => 0)
                .RuleFor(p => p.AverageTrainingHoursPerWeek, f => f.Random.Decimal(5, 25))
                .RuleFor(p => p.InjuryHistory, f => f.Lorem.Sentences(f.Random.Int(0, 3)))
                .RuleFor(p => p.CareerHighlights, f => f.Lorem.Sentences(f.Random.Int(1, 3)));

            _athlete = _faker.Generate();
        }

        public AthleteBuilder WithEmail(string email)
        {
            _athlete.Email = email;
            return this;
        }

        public AthleteBuilder WithUsername(string username)
        {
            _athlete.UserName = username;
            return this;
        }

        public AthleteBuilder WithWinRate(decimal winRate)
        {
            _athlete.WinRate = Math.Clamp(winRate, 0, 1);
            return this;
        }

        public AthleteBuilder WithRankingPoints(decimal points)
        {
            _athlete.RankingPoints = points;
            return this;
        }

        public AthleteBuilder WithCountry(string country)
        {
            _athlete.Country = country;
            return this;
        }

        public AthleteBuilder WithCity(string city)
        {
            _athlete.City = city;
            return this;
        }

        public AthleteBuilder WithPrimarySport(string sport)
        {
            _athlete.PrimarySport = sport;
            return this;
        }

        public AthleteBuilder WithMatchesPlayed(int matches)
        {
            _athlete.MatchesPlayed = matches;
            _athlete.MatchesWon = (int)(matches * (double)_athlete.WinRate);
            _athlete.MatchesLost = matches - _athlete.MatchesWon;
            return this;
        }

        public Athlete Build()
        {
            return _athlete;
        }
    }
}
