using FAIR.API.Middleware;
using FAIR.Application.Services.Interfaces.Logging;

namespace FAIR.API.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        internal static void ConfigureExceptionHandler(this IApplicationBuilder app, IAppLogger<Program> logger)
        {
            logger.LogInformation("Configuring global exception middleware.");
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
