namespace Modsen.Portal.ModsenApi.Contract;

public class CertificateNotFoundException : Exception
{
    public CertificateNotFoundException() { }

    public CertificateNotFoundException(string message) : base(message) { }

    public CertificateNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
