using FAIR.Application.DTOs.Search;
using FluentValidation;

namespace FAIR.Application.Validations.Search
{
    public class AthleteSearchFilterValidator : AbstractValidator<AthleteSearchFilter>
    {
        public AthleteSearchFilterValidator()
        {
            RuleFor(x => x.Location).MaximumLength(200);
            RuleFor(x => x.PrimarySport).MaximumLength(100);
            RuleFor(x => x.MinAge).InclusiveBetween(1, 100).When(x => x.MinAge.HasValue);
            RuleFor(x => x.MaxAge).InclusiveBetween(1, 100).When(x => x.MaxAge.HasValue);
            RuleFor(x => x.MinAge)
                .LessThanOrEqualTo(x => x.MaxAge!.Value)
                .When(x => x.MinAge.HasValue && x.MaxAge.HasValue);
            RuleFor(x => x.MinWinRate).InclusiveBetween(0, 100).When(x => x.MinWinRate.HasValue);
            RuleFor(x => x.MaxWinRate).InclusiveBetween(0, 100).When(x => x.MaxWinRate.HasValue);
            RuleFor(x => x.MinWinRate)
                .LessThanOrEqualTo(x => x.MaxWinRate!.Value)
                .When(x => x.MinWinRate.HasValue && x.MaxWinRate.HasValue);
            RuleFor(x => x.MinRankingPoints).GreaterThanOrEqualTo(0).When(x => x.MinRankingPoints.HasValue);
            RuleFor(x => x.MaxRankingPoints).GreaterThanOrEqualTo(0).When(x => x.MaxRankingPoints.HasValue);
            RuleFor(x => x.MinRankingPoints)
                .LessThanOrEqualTo(x => x.MaxRankingPoints!.Value)
                .When(x => x.MinRankingPoints.HasValue && x.MaxRankingPoints.HasValue);
            RuleFor(x => x.MinSkillScore).GreaterThanOrEqualTo(0).When(x => x.MinSkillScore.HasValue);
        }
    }
}
