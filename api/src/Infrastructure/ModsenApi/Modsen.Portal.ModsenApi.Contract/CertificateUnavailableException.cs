namespace Modsen.Portal.ModsenApi.Contract;

public sealed class CertificateUnavailableException : Exception
{
    public CertificateUnavailableException() { }

    public CertificateUnavailableException(string message) : base(message) { }

    public CertificateUnavailableException(string message, Exception innerException)
        : base(message, innerException) { }
}
