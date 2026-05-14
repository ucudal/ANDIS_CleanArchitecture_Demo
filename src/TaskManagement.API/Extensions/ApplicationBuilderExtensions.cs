namespace TaskManagement.API.Extensions;

using Microsoft.AspNetCore.Builder;
using TaskManagement.API.Middleware;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}