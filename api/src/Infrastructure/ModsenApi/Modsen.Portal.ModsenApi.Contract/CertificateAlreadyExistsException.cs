namespace Modsen.Portal.ModsenApi.Contract;

public sealed class CertificateAlreadyExistsException : Exception
{
    public CertificateAlreadyExistsException() { }

    public CertificateAlreadyExistsException(string message) : base(message) { }

    public CertificateAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException) { }
}
