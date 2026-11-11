using System.Net.Mime;
using System.Text.Json;
using Microsoft.OData;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Authentication.External;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Jira.Contract.Exceptions;
using Modsen.Portal.ModsenApi.Contract;

namespace Modsen.Portal.Api.Middleware;

internal sealed class ErrorHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            int statusCode;
            var logger = context.RequestServices.GetRequiredService<ILogger<ErrorHandlerMiddleware>>();
            var correlationContext = context.RequestServices.GetRequiredService<ICorrelationContext>();
            var hostEnvContext = context.RequestServices.GetRequiredService<IHostEnvironmentContext>();
            var errorInfo = hostEnvContext.IsDevelopmentOrQaEnvironment()
                ? ex.Message
                : $"Unexpected error. Correlation ID: {correlationContext.CorrelationId}";
            var exceptionResponse = new ExceptionResponse(errorInfo)
            {
                Message = "Something went wrong. Please try again later."
            };

            logger.LogError(ex, "Unexpected error {CorrelationId}: {Message}", correlationContext.CorrelationId, ex.Message);

            switch (ex)
            {
                case UnauthorizedException:
                case ExternalUnauthorizedException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    break;

                case CertificateAlreadyExistsException:
                case EntityConflictException:
                    exceptionResponse.Message = ex.Message;
                    statusCode = StatusCodes.Status409Conflict;
                    break;

                case ODataException:
                case InvalidDataAppException:
                case JiraUnauthorizedException:
                    exceptionResponse.Message = ex.Message;
                    statusCode = StatusCodes.Status400BadRequest;
                    break;

                case CertificateUnavailableException:
                case JiraCredentialsCheckFailedException:
                    statusCode = StatusCodes.Status400BadRequest;
                    break;

                case EntityNotFoundException:
                case CertificateNotFoundException:
                    exceptionResponse.Message = ex.Message;
                    statusCode = StatusCodes.Status404NotFound;
                    break;

                case ActionNotAllowedException:
                    exceptionResponse.Message = ex.Message;
                    statusCode = StatusCodes.Status403Forbidden;
                    break;

                case RequestValidationFailedException validationFailedException:
                    statusCode = StatusCodes.Status400BadRequest;
                    exceptionResponse.ValidationFailures = validationFailedException.Failures;
                    break;

                case ValidationFailedException:
                    statusCode = StatusCodes.Status400BadRequest;
                    exceptionResponse.ValidationFailures = [];
                    break;

                case TaskCanceledException:
                case OperationCanceledException:
                    exceptionResponse.Message = PortalMessages.OperationCanceledMessage;
                    statusCode = StatusCodes.Status202Accepted;
                    break;

                case JiraTemporarilyUnavailableException:
                    exceptionResponse.Message = ex.Message;
                    statusCode = StatusCodes.Status503ServiceUnavailable;
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            var json = JsonSerializer.Serialize(exceptionResponse);

            await context.Response.WriteAsync(json, context.RequestAborted);
        }
    }
}
