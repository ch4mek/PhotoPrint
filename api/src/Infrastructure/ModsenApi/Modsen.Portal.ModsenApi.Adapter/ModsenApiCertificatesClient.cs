using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.ModsenApi.Adapter.ApiModels;
using Modsen.Portal.ModsenApi.Adapter.Mapping;
using Modsen.Portal.ModsenApi.Contract;

namespace Modsen.Portal.ModsenApi.Adapter;

public sealed class ModsenApiCertificatesClient : IModsenApiCertificatesClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ModsenApiCertificatesClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<CertificateModel>> GetUserCertificates(
        string userId,
        CancellationToken cancellationToken)
    {
        var certificatesHttpClient = GetCertificatesHttpClient();
        var url = new Uri($"/api/certificates/{userId}", UriKind.Relative);
        var response = await certificatesHttpClient.GetAsync(url, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ListOfCertificatesApiModel>(
                cancellationToken: cancellationToken);
            if (result == null)
            {
                throw new AppException("Received empty response from Modsen API (Get User Certificates) or unexpected message format");
            }

            if (!result.IsSuccess)
            {
                throw new AppException("Received failed response from Modsen API (Get User Certificates)");
            }

            return result.Certificates
                .Select(x => x.Map(certificatesHttpClient.BaseAddress!.ToString()))
                .ToList();
        }

        throw new AppException($"HTTP Error during getting information from Modsen API (Get User Certificates). Reason: {response.ReasonPhrase}");
    }

    public async Task<CertificateModel> CreateUserCertificate(
        CreateCertificateModel model,
        string dateFormat,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        var certificatesHttpClient = GetCertificatesHttpClient();

        var jsonModel = !model.Stack.IsNullOrWhiteSpace()
            ? JsonSerializer.Serialize(model.MapWithStack(dateFormat))
            : JsonSerializer.Serialize(model.MapWithoutStack(dateFormat));

        using var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");

        var url = new Uri("/api/certificates", UriKind.Relative);
        var healthCheckUrl = new Uri("/api/healthcheck", UriKind.Relative);

        var certificatesHealthCheckHttpClient = GetCertificatesHealthCheckHttpClient();

        await certificatesHealthCheckHttpClient.GetAsync(healthCheckUrl, cancellationToken);

        var response = await certificatesHttpClient.PostAsync(url, content, cancellationToken);

        return await EnsureCertificateFromApiAsync(
            response,
            apiName: "Create User Certificate",
            cancellationToken);
    }

    public async Task<CertificateModel> DeleteUserCertificate(string certificateId, CancellationToken cancellationToken)
    {
        var certificatesHttpClient = GetCertificatesHttpClient();
        var model = new { certificateId };

        using var request = new HttpRequestMessage();

        request.RequestUri = new Uri("/api/certificates", UriKind.Relative);
        request.Method = HttpMethod.Delete;
        request.Content = JsonContent.Create(model);

        var response = await certificatesHttpClient.SendAsync(request, cancellationToken);

        return await EnsureCertificateFromApiAsync(
            response,
            apiName: "Delete User Certificate",
            cancellationToken);
    }

    private static async Task<CertificateModel> EnsureCertificateFromApiAsync(
        HttpResponseMessage response,
        string apiName,
        CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw response.StatusCode switch
            {
                HttpStatusCode.Conflict => new CertificateAlreadyExistsException(),
                HttpStatusCode.BadRequest => new CertificateUnavailableException(),
                HttpStatusCode.NotFound => new CertificateNotFoundException(),
                _ => new AppException(
                    $"HTTP Error during requesting certificate using Modsen API ({apiName}). Reason: {response.ReasonPhrase}")
            };
        }

        var result = await response.Content.ReadFromJsonAsync<ResponseCertificateApiModel>(
            cancellationToken: cancellationToken);
        if (result?.Certificate == null)
        {
            throw new AppException($"Received empty response from Modsen API ({apiName}) or unexpected message format");
        }

        if (!result.IsSuccess)
        {
            throw new AppException($"Received failed response from Modsen API ({apiName})");
        }

        return result.Certificate.Map();

    }

    private HttpClient GetCertificatesHttpClient()
    {
        return _httpClientFactory.CreateClient(ModsenApiConstants.CertificatesHttpClientName);
    }

    private HttpClient GetCertificatesHealthCheckHttpClient()
    {
        return _httpClientFactory.CreateClient(ModsenApiConstants.CertificatesHealthCheckHttpClientName);
    }
}
