namespace Kontur.Core.Exceptions;

/// <summary>Доменная ошибка с HTTP-кодом; маппится в ProblemDetails (RFC 7807, ОТ-05).</summary>
public class ApiException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
