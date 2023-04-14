using FluentValidation;
using MediatR;

namespace Core.Application;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return next();
        }

        var context = new ValidationContext<TRequest>(request);

        var errors = _validators
            .Select(validator => validator.Validate(context))
            .SelectMany(validationResult => validationResult.Errors)
            .Where(errors => errors != null)
            .ToLookup(failure => failure.PropertyName)
            .ToDictionary(
                //Manually transform to camel case due to a bug during serialization for dictionary keys
                //https://github.com/dotnet/runtime/issues/30008
                failures => $"{char.ToLowerInvariant(failures.Key[0])}{failures.Key.Substring(1)}",
                failures => failures.Select(failure => new ValidationErrorDetail(failure.ErrorCode, failure.FormattedMessagePlaceholderValues?.Select(x => x.Value).ToArray())));

        if (errors.Count != 0)
        {
            throw new ValidationException(errors);
        }

        return next();
    }
}
