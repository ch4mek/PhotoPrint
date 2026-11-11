using System.Net.Http.Json;
using Modsen.Portal.Certificate.UseCases.Dto;
using Modsen.Portal.Core.Certificate;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;

internal static class HttpClientCertificatesExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static async Task<PageableListOfItems<CertificateDto>?> GetEmployeeCertificates(this HttpClient client,
        int id,
        int top = 25,
        int skip = 0)
    {
        return await client.GetFromJsonAsync<PageableListOfItems<CertificateDto>>(
            $"api/Certificate/{id}?top={top}&skip={skip}",
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> DeleteCertificateAsHttpMessage(this HttpClient client, string id)
    {
        return await client.DeleteAsync(
            $"api/Certificate/{id}",
            _cancellationToken);
    }


    public static async Task<HttpResponseMessage> CreateCertificateAsHttpMessage(this HttpClient client,
        string recipientId,
        string recipient = "",
        DateTime? receiptDate = null,
        string technology = "Net",
        JsStackType? stack = null)
    {
        var certificateToCreate = new CreateCertificateDto
        {
            RecipientId = recipientId,
            Recipient = recipient.IsNullOrEmpty() ? $"{Guid.CreateVersion7().ToAlphabetic()} {Guid.CreateVersion7().ToAlphabetic()}" : recipient,
            ReceiptDate = receiptDate ?? DateTime.UtcNow,
            Technology = technology,
            Stack = stack
        };

        return await client.PostAsJsonAsync(
            "api/Certificate",
            certificateToCreate,
            _cancellationToken);
    }

    public static async Task<CertificateDto?> CreateCertificate(this HttpClient client,
        string recipientId,
        string recipient = "",
        DateTime? receiptDate = null,
        string technology = "Net",
        JsStackType? stack = null)
    {
        var certificateToCreate = new CreateCertificateDto
        {
            RecipientId = recipientId,
            Recipient = recipient.IsNullOrEmpty() ? $"{Guid.CreateVersion7().ToAlphabetic()} {Guid.CreateVersion7().ToAlphabetic()}" : recipient,
            ReceiptDate = receiptDate ?? DateTime.UtcNow,
            Technology = technology,
            Stack = stack
        };

        return await client.PostAsJsonAsync<CreateCertificateDto, CertificateDto>(
            "api/Certificate",
            certificateToCreate,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> CreateCertificateAsHttpMessageAsync(this HttpClient client, CreateCertificateDto input)
    {
        return await client.PostAsJsonAsync(
            "api/Certificate",
            input,
            _cancellationToken);
    }
}
