using FluentValidation;
using MediatR;
using Template.Application.Common.Errors;
using Template.Application.Common.Results;

namespace Template.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var errors = failures.Select(f =>
            new Error(
                Code: "validation." + (f.PropertyName ?? "unknown").ToLowerInvariant(),
                Message: f.ErrorMessage,
                Type: ErrorType.Validation,
                Field: f.PropertyName
            )).ToList();

        object resultObj = typeof(TResponse) switch
        {
            var t when t == typeof(Result) => Result.Failure(errors),
            var t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>)
                => CreateGenericFailure(t, errors),
            _ => throw new ValidationException(failures)
        };

        return (TResponse)resultObj;
    }

    private static object CreateGenericFailure(Type resultType, List<Error> errors)
    {
        var failureMethod = resultType.GetMethod(
            "Failure",
            [typeof(List<Error>)]
        );

        if (failureMethod is null)
            throw new InvalidOperationException("Result<T>.Failure(List<Error>) not found.");

        return failureMethod.Invoke(null, [errors])!;
    }
}
