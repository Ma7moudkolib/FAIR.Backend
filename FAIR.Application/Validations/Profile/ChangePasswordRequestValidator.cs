using FAIR.Application.DTOs.Profile;
using FluentValidation;

namespace FAIR.Application.Validations.Profile
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Current password is required.");
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .Matches("[A-Z]").WithMessage("Password must be at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must be at least one lowercase letter.")
                .Matches("\\d").WithMessage("Password must be at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must be at least one special character.");
        }
    }
}
