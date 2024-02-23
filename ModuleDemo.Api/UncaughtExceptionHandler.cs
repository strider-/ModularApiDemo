using Microsoft.AspNetCore.Diagnostics;

namespace ModuleDemo.Api;

public static class UncaughtExceptionHandler
{
    public static async Task Handle(HttpContext ctx)
    {
        var exceptionDetails = ctx.Features.Get<IExceptionHandlerFeature>();
        var statusCode = 500;
        var title = "An unexpected error occurred while processing your request";

        if (exceptionDetails?.Error is BadHttpRequestException br)
        {
            statusCode = br.StatusCode;
            title = br.InnerException?.Message ?? br.Message;
        }

        await TypedResults.Problem(statusCode: statusCode, title: title).ExecuteAsync(ctx);
    }
}