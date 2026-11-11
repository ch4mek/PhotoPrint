namespace Modsen.Portal.ModsenApi.Contract;

public interface IModsenApiCertificatesClient
{
    Task<List<CertificateModel>> GetUserCertificates(string userId, CancellationToken cancellationToken);
    Task<CertificateModel> CreateUserCertificate(CreateCertificateModel model,
        string dateFormat, CancellationToken cancellationToken);
    Task<CertificateModel> DeleteUserCertificate(string certificateId, CancellationToken cancellationToken);
}
