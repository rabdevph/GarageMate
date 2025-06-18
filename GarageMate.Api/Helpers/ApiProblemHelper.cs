using System.ComponentModel.DataAnnotations;

namespace GarageMate.Api.Helpers;

public class ApiProblemHelper
{
    public static IResult CreateProblem(
        string type,
        string title,
        int statusCode,
        string detail,
        string instance
    )
    {
        return Results.Problem(
            type: type,
            title: title,
            statusCode: statusCode,
            detail: detail,
            instance: instance
        );
    }

    public static IResult CreateProblem(
        string type,
        string title,
        int statusCode,
        string detail,
        string instance,
        IEnumerable<ValidationResult> validationResults
    )
    {
        var groupErrors = validationResults
            .GroupBy(v => v.MemberNames.FirstOrDefault() ?? "general")
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage ?? "Invalid value").ToArray()
            );

        var extensions = new Dictionary<string, object?> { ["errors"] = groupErrors };

        return Results.Problem(
            type: type,
            title: title,
            statusCode: statusCode,
            detail: detail,
            instance: instance,
            extensions: extensions
        );
    }
}
