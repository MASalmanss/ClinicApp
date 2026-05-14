using ClinicApp.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Api.Extensions;

public static class ResultExtensions
{
    // Result<T> → IActionResult
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return result.Error!.Type switch
        {
            ErrorType.NotFound    => new NotFoundObjectResult(new { error = result.Error.Message }),
            ErrorType.Conflict    => new ConflictObjectResult(new { error = result.Error.Message }),
            ErrorType.Validation  => new BadRequestObjectResult(new { error = result.Error.Message }),
            _                     => new ObjectResult(new { error = result.Error.Message }) { StatusCode = 500 }
        };
    }

    // Non-generic Result → IActionResult (204 No Content başarıda)
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
            return new NoContentResult();

        return result.Error!.Type switch
        {
            ErrorType.NotFound    => new NotFoundObjectResult(new { error = result.Error.Message }),
            ErrorType.Conflict    => new ConflictObjectResult(new { error = result.Error.Message }),
            ErrorType.Validation  => new BadRequestObjectResult(new { error = result.Error.Message }),
            _                     => new ObjectResult(new { error = result.Error.Message }) { StatusCode = 500 }
        };
    }
}
