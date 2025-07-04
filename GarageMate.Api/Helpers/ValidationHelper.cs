using System.ComponentModel.DataAnnotations;

namespace GarageMate.Api.Helpers;

public class ValidationHelper
{
    public static IResult ValidateDto<T>(T model, HttpRequest request)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model!);

        if (!Validator.TryValidateObject(model!, validationContext, validationResults))
        {
            return ApiProblemHelper.CreateProblem(
                "urn:problem-type:validation-error",
                "Validation Failed",
                StatusCodes.Status400BadRequest,
                "One or more fields failed validation.",
                request.Path,
                validationResults
            );
        }

        return null!;
    }

    public static IResult ValidateDuplicateRecord(bool isDuplicate, string fieldName, HttpRequest request)
    {
        if (isDuplicate)
        {
            var kebabFieldName = fieldName.ToLower().Replace(" ", "-");

            return ApiProblemHelper.CreateProblem(
                $"urn:problem-type:duplicate-{kebabFieldName}",
                $"Duplicate {fieldName}",
                StatusCodes.Status409Conflict,
                $"An entry with the same {fieldName} already exists.",
                request.Path
             );
        }

        return null!;
    }

    public static IResult ValidateNotFound(object? resource, string resourceName, HttpRequest request)
    {
        if (resource is null)
        {
            var kebabResourceName = resourceName.ToLower().Replace(" ", "-");

            return ApiProblemHelper.CreateProblem(
                $"urn:problem-type:{kebabResourceName}-not-found",
                $"{resourceName} Not Found",
                StatusCodes.Status404NotFound,
                $"The requested {resourceName.ToLower()} could not be found.",
                request.Path
             );
        }

        return null!;
    }
}
