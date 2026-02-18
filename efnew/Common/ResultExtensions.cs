using Microsoft.AspNetCore.Mvc;

namespace Common;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.StatusCode(result.StatusCode);

        return controller.StatusCode(result.StatusCode, result.Error);
    }

    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.StatusCode(result.StatusCode, result.Data);

        return controller.StatusCode(result.StatusCode, result.Error);
    }
}
