using FAIR.Application.DTOs.Profile;
using FluentValidation;

namespace FAIR.Application.Validations.Profile
{
    public class UpdateCoachProfileValidator : AbstractValidator<UpdateCoachProfile>
    {
        public UpdateCoachProfileValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Coach Id is required.");
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Specialization).NotEmpty().MaximumLength(100);
            RuleFor(x => x.YearsOfExperience).InclusiveBetween(0, 70);
            RuleFor(x => x.Certifications).MaximumLength(2000);
            RuleFor(x => x.CoachingLicenseLevel).MaximumLength(100);
            RuleFor(x => x.PreferredTrainingMethodology).MaximumLength(1000);
            RuleFor(x => x.TeamOrOrganization).MaximumLength(200);
            RuleFor(x => x.AthletesCoachedCount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CareerWinRate).InclusiveBetween(0, 100);
        }
    }
}
