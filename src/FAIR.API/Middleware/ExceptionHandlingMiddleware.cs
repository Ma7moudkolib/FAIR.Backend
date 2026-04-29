using System.Text.Json;
using FAIR.Application.Exceptions;
using FAIR.Application.Services.Interfaces.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FAIR.API.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, IAppLogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (DbUpdateException ex)
            {
                await HandleDatabaseExceptionAsync(context, ex);
            }
            catch (BadHttpRequestException ex)
            {
                var statusCode = ex.StatusCode == StatusCodes.Status413PayloadTooLarge
                    ? StatusCodes.Status413PayloadTooLarge
                    : StatusCodes.Status400BadRequest;
                await WriteResponseAsync(context, statusCode, ex.Message);
            }
            catch (ServiceValidationException ex)
            {
                await WriteValidationResponseAsync(context, ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception");
                await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        private async Task HandleDatabaseExceptionAsync(HttpContext context, DbUpdateException ex)
        {
            if (ex.InnerException is not SqlException sqlException)
            {
                logger.LogError(ex, "Database update exception");
                await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, "Database update failed.");
                return;
            }

            logger.LogError(sqlException, "SQL exception");

            var (statusCode, message) = sqlException.Number switch
            {
                2627 => (StatusCodes.Status409Conflict, "Unique constraint violation."),
                515 => (StatusCodes.Status400BadRequest, "Required value was null."),
                547 => (StatusCodes.Status409Conflict, "Foreign key constraint violation."),
                _ => (StatusCodes.Status500InternalServerError, "Database error while processing the request.")
            };

            await WriteResponseAsync(context, statusCode, message);
        }

        private static async Task WriteResponseAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new { error = message });
            await context.Response.WriteAsync(payload);
        }

        private static async Task WriteValidationResponseAsync(HttpContext context, ServiceValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new
            {
                message = "Validation failed",
                errors = ex.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage })
            });
            await context.Response.WriteAsync(payload);
        }
    }
}
