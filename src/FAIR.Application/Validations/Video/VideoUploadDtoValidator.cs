using FAIR.Application.DTOs.Video;
using FluentValidation;

namespace FAIR.Application.Validations.Video
{
    public class VideoUploadDtoValidator : AbstractValidator<VideoUploadDto>
    {
        private const long MaxBytes = 536_870_912;

        public VideoUploadDtoValidator()
        {
            RuleFor(x => x.Video)
                .NotNull().WithMessage("Video file is required.")
                .Must(v => v != null && v.Length > 0).WithMessage("Video file cannot be empty.")
                .Must(v => v != null && v.Length <= MaxBytes).WithMessage("Video file is too large.");
        }
    }
}
