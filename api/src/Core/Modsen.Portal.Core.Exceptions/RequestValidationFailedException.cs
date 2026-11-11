using FluentValidation.Results;

namespace Modsen.Portal.Core.Exceptions;

public sealed class RequestValidationFailedException : Exception
{
    public IReadOnlyCollection<ValidationFailure> Failures { get; set; } = [];

    public RequestValidationFailedException() { }

    public RequestValidationFailedException(string message) : base(message) { }

    public RequestValidationFailedException(string message, Exception innerException) : base(message, innerException) { }
}
