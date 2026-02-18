namespace Template.Application.Common.Errors;

public sealed record Error(
    string Code,
    string Message,
    ErrorType Type,
    string? Field = null
);
