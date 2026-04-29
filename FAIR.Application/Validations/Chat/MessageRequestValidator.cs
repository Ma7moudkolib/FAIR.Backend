using FAIR.Application.DTOs.Chat;
using FluentValidation;

namespace FAIR.Application.Validations.Chat
{
    public class MessageRequestValidator : AbstractValidator<MessageRequest>
    {
        public MessageRequestValidator()
        {
            RuleFor(x => x.Content).NotEmpty().WithMessage("Message content is required.").MaximumLength(2000);
            RuleFor(x => x.SenderId).NotEmpty().WithMessage("SenderId is required.");
            RuleFor(x => x.ReceiverId).NotEmpty().WithMessage("ReceiverId is required.");
        }
    }
}
