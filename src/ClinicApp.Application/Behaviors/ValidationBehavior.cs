using ClinicApp.Application.Common;
using FluentValidation;
using MediatR;

namespace ClinicApp.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var errorMessage = string.Join(" | ", failures.Select(f => f.ErrorMessage));

        // Result<T> veya Result döndüren tüm handler'lar için çalışır
        var error = Error.Validation(errorMessage);

        // TResponse = Result<T>
        var resultType = typeof(TResponse);
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var innerType = resultType.GetGenericArguments()[0];
            var failMethod = resultType.GetMethod("Fail")!;
            return (TResponse)failMethod.Invoke(null, [error])!;
        }

        // TResponse = Result (non-generic)
        if (resultType == typeof(Result))
            return (TResponse)(object)Result.Fail(error);

        throw new ValidationException(failures);
    }
}
