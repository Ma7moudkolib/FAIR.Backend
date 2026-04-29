using FAIR.Application.DTOs.Profile;
using FluentValidation;

namespace FAIR.Application.Validations.Profile
{
    public class UpdateAthleteProfileValidator : AbstractValidator<UpdateAthleteProfile>
    {
        public UpdateAthleteProfileValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Athlete Id is required.");
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DateOfBirth).LessThan(DateOnly.FromDateTime(DateTime.UtcNow));
            RuleFor(x => x.Weight).InclusiveBetween(0, 250);
            RuleFor(x => x.Height).InclusiveBetween(0, 250);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Country).MaximumLength(100);
            RuleFor(x => x.City).MaximumLength(100);
            RuleFor(x => x.DominantHand).MaximumLength(30);
            RuleFor(x => x.BodyFatPercentage).InclusiveBetween(0, 100);
            RuleFor(x => x.Wingspan).InclusiveBetween(0, 300);
            RuleFor(x => x.Reach).InclusiveBetween(0, 300);
            RuleFor(x => x.PrimarySport).MaximumLength(100);
            RuleFor(x => x.CurrentClub).MaximumLength(150);
            RuleFor(x => x.CareerStartYear).InclusiveBetween(1900, 2100);
            RuleFor(x => x.YearsOfProfessionalExperience).InclusiveBetween(0, 60);
            RuleFor(x => x.MatchesPlayed).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MatchesWon).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MatchesLost).GreaterThanOrEqualTo(0);
            RuleFor(x => x.WinRate).InclusiveBetween(0, 100);
            RuleFor(x => x.RankingPoints).InclusiveBetween(0, 1000000);
            RuleFor(x => x.AverageTrainingHoursPerWeek).InclusiveBetween(0, 168);
            RuleFor(x => x.InjuryHistory).MaximumLength(2000);
            RuleFor(x => x.CareerHighlights).MaximumLength(2000);
        }
    }
}
