using Template.Application.Common.Errors;

namespace Template.Application.Common.Results;

public class Result
{
    public bool IsSuccess { get; }
    public List<Error> Errors { get; } = [];

    protected Result(bool isSuccess, List<Error>? errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? [];
    }

    public static Result Success() => new(true, null);

    public static Result Failure(params Error[] errors) => new(false, errors.ToList());
    public static Result Failure(List<Error> errors) => new(false, errors);
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, List<Error>? errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null);

    public static new Result<T> Failure(params Error[] errors)
        => new(false, default, errors.ToList());

    public static Result<T> Failure(List<Error> errors)
        => new(false, default, errors);
}
