using FluentValidation.Results;

namespace FAIR.Application.Exceptions
{
    public class ServiceValidationException : Exception
    {
        public IReadOnlyList<ValidationFailure> Errors { get; }

        public ServiceValidationException(IEnumerable<ValidationFailure> errors)
            : base("Validation failed")
        {
            Errors = errors.ToList().AsReadOnly();
        }
    }
}
