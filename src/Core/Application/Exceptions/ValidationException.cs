using Core.Domain;

namespace Core.Application;

public class ValidationException : BaseException
{
    public ValidationException(Dictionary<string, IEnumerable<ValidationErrorDetail>> errors)
        : base("ValidationErrorDetail") => Errors = errors;

    public IDictionary<string, IEnumerable<ValidationErrorDetail>> Errors { get; }
}
