using FluentValidation.Results;

namespace Modsen.Portal.Core.Api.Response;

public class ExceptionResponse
{
    public ExceptionResponse(string errorInfo)
    {
        ErrorInfo = errorInfo;
        ValidationFailures = [];
    }

    public string? Message { get; set; }
    public string ErrorInfo { get; init; }
    public IReadOnlyCollection<ValidationFailure> ValidationFailures { get; set; }
}
