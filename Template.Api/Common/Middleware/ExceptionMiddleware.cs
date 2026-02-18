using Microsoft.AspNetCore.Mvc;
using Template.Application.Common.Errors;
using Template.Application.Common.Results;

namespace Template.Api.Common.Middleware;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.IsSuccess) return controller.Ok();

        return controller.ProblemFromErrors(result.Errors);
    }

    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess) return controller.Ok(result.Value);

        return controller.ProblemFromErrors(result.Errors);
    }

    private static IActionResult ProblemFromErrors(this ControllerBase controller, List<Error> errors)
    {
        var status = MapStatus(errors);
        var validation = errors.Where(e => e.Type == ErrorType.Validation).ToList();

        if (validation.Count != 0)
        {
            var modelState = new Dictionary<string, string[]>();

            foreach (var grp in validation.GroupBy(e => e.Field ?? ""))
                modelState[grp.Key] = grp.Select(x => x.Message).ToArray();

            var problemDetails = new ValidationProblemDetails(modelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            };

            return controller.BadRequest(problemDetails);
        }


        var first = errors.FirstOrDefault();
        return controller.Problem(
            title: first?.Code ?? "error",
            detail: first?.Message ?? "Something went wrong",
            statusCode: status
        );
    }

    private static int MapStatus(List<Error> errors)
    {
        var e = errors.FirstOrDefault(x => x.Type != ErrorType.Validation) ?? errors.First();

        return e.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };
    }
}
