using Microsoft.EntityFrameworkCore;

namespace GarageMate.Api.Helpers;

public class DbContextHelper
{
    public static async Task<IResult> TrySaveChangesAsync(
           DbContext dbContext,
           HttpContext httpContext,
           Func<Task<IResult>> onSuccessAsync
       )
    {
        try
        {
            await dbContext.SaveChangesAsync();
            return await onSuccessAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return ApiProblemHelper.CreateProblem(
                "urn:problem-type:concurrency-error",
                "Concurrency Conflict",
                StatusCodes.Status409Conflict,
                "The entry was modified by another operation. Please try again.",
                httpContext.Request.Path
            );
        }
        catch (DbUpdateException)
        {
            return ApiProblemHelper.CreateProblem(
                "urn:problem-type:database-update-failed",
                "Database Update Failed",
                StatusCodes.Status409Conflict,
                "Database error occured. Possible duplicate value or constraint issue.",
                httpContext.Request.Path
            );
        }
        catch (OperationCanceledException)
        {
            return ApiProblemHelper.CreateProblem(
                "urn:problem-type:operation-canceled",
                "Operation Cancelled",
                StatusCodes.Status499ClientClosedRequest,
                "The operation was canceled, possibly due to a timeout or client disconnect.",
                httpContext.Request.Path
            );
        }
        catch (Exception)
        {
            return ApiProblemHelper.CreateProblem(
                "urn:problem-type:internal-server-error",
                "Internal Server Error",
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred while processing your request.",
                httpContext.Request.Path
            );
        }
    }
}
