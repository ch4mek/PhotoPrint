using System.Net.Http.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Equipment.Controllers.Contracts.Input;
using Modsen.Portal.Equipment.Controllers.Contracts.Output;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;

internal static class HttpClientEquipmentExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> UpdateEquipmentAsResponseMessageAsync(
        this HttpClient httpClient,
        UpdateEquipmentRequestDto updateEquipmentInput)
    {
        return httpClient.PutAsJsonAsync(
            "api/Equipment",
            updateEquipmentInput,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateEquipmentAsResponseMessageAsync(
        this HttpClient httpClient,
        CreateEquipmentRequestDto createEquipmentInput)
    {
        return httpClient.PostAsJsonAsync(
            "api/Equipment",
            createEquipmentInput,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateEquipmentAssignAsResponseMessageAsync(
        this HttpClient httpClient,
        AssignEquipmentInput assignEquipmentInput,
        int id)
    {
        return httpClient.PostAsJsonAsync(
            $"api/EquipmentHistory/{id}/assign",
            assignEquipmentInput,
            _cancellationToken);
    }

    public static Task<EquipmentOutput?> CreateEquipmentAsync(
        this HttpClient httpClient,
        CreateEquipmentRequestDto createEquipmentInput)
    {
        return httpClient.PostAsJsonAsync<CreateEquipmentRequestDto, EquipmentOutput>(
            "api/Equipment",
            createEquipmentInput,
            _cancellationToken);
    }

    public static Task<PageableListOfItems<OdpEquipmentHolder>?> GetEquipmentHistoryAsync(
        this HttpClient httpClient,
        int id,
        int top,
        int skip,
        string filter)
    {
        return httpClient.GetFromJsonAsync<PageableListOfItems<OdpEquipmentHolder>?>(
            $"api/EquipmentHistory/{id}/history?top={top}&skip={skip}&filter={filter}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<EquipmentOutput?> GetEquipmentAsync(
        this HttpClient httpClient,
        int id)
    {
        return httpClient.GetFromJsonAsync<EquipmentOutput>(
            $"api/Equipment/{id}", IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetEquipmentAsResponseMessageAsync(
        this HttpClient httpClient,
        int id)
    {
        return httpClient.GetAsync(
            $"api/Equipment/{id}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetEquipmentByFileAsResponseMessageAsync(
        this HttpClient httpClient,
        int id)
    {
        return httpClient.GetAsync(
            $"api/Equipment/file/{id}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetEquipmentByFileAsResponseMessageAsync(
        this HttpClient httpClient,
        int id,
        int? top = null,
        int? skip = null)
    {
        return httpClient.GetAsync(
            $"api/Equipment/file/{id}?top={top}&skip={skip}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetPageableEquipmentAsResponseMessageAsync(
        this HttpClient httpClient,
        int top,
        int skip,
        int id)
    {
        return httpClient.GetAsync(
            $"api/Equipment?skip={skip}&top={top}&$filter=Id eq {id}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetPageableEquipmentAsResponseMessageAsync(
        this HttpClient httpClient,
        int top,
        int skip)
    {
        return httpClient.GetAsync(
            $"api/Equipment?skip={skip}&top={top}",
            _cancellationToken);
    }

    public static Task<PageableListOfItems<EquipmentOutput>?> GetPageableEquipmentAsync(
        this HttpClient httpClient,
        int top,
        int skip,
        int approverId,
        string equipmentTypeId)
    {
        return httpClient.GetFromJsonAsync<PageableListOfItems<EquipmentOutput>>(
            $"api/Equipment?skip={skip}&top={top}&$filter=ApproverId eq {approverId} and EquipmentTypeId eq '{equipmentTypeId}'",
            IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<PageableListOfItems<EquipmentOutput>?> GetPageableEquipmentAsync(
        this HttpClient httpClient,
        int id,
        int top,
        int skip)
    {
        return httpClient.GetFromJsonAsync<PageableListOfItems<EquipmentOutput>>(
            $"api/Equipment?skip={skip}&top={top}&$filter=Id eq {id}",
            IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<PageableListOfItems<EquipmentOutput>?> GetPageableEquipmentAsync(
        this HttpClient httpClient,
        int top,
        int skip)
    {
        return httpClient.GetFromJsonAsync<PageableListOfItems<EquipmentOutput>>(
            $"api/Equipment?skip={skip}&top={top}",
            IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<PageableListOfItems<EquipmentOutput>?> GetPageableEquipmentAsync(
        this HttpClient httpClient,
        int top,
        int skip,
        string serialNumber)
    {
        return httpClient.GetFromJsonAsync<PageableListOfItems<EquipmentOutput>>(
            $"api/Equipment?top={top}&skip={skip}&filter=startswith(serialNumber,'{serialNumber}')",
            IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<PageableListOfItems<EquipmentOutput>?> GetPageableEquipmentWithEqSerialNumberAsync(
       this HttpClient httpClient,
       int top,
       int skip,
       string serialNumber)
    {
        return httpClient.GetFromJsonAsync<PageableListOfItems<EquipmentOutput>>(
            $"api/Equipment?top={top}&skip={skip}&filter=serialNumber eq '{serialNumber}'",
            IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetPageableEquipmentWithQueryAsResponseMessageAsync(
        this HttpClient httpClient,
        string query)
    {
        return httpClient.GetAsync(new Uri($"api/Equipment{query}", UriKind.Relative), _cancellationToken);
    }


    public static Task<PageableListOfItems<EquipmentOutput>?> GetPageableEquipmentWithFilterAsync(
        this HttpClient httpClient,
        int top,
        int skip,
        string filter)
    {
        return httpClient.GetFromJsonAsync<PageableListOfItems<EquipmentOutput>>(
            $"api/Equipment?top={top}&skip={skip}&filter={filter}",
            IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteEquipmentHistoryAsync(
        this HttpClient httpClient,
        int id)
    {
        return httpClient.DeleteAsync(
            $"api/EquipmentHistory/{id}/history",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteEquipmentHistoryByIdAsync(
        this HttpClient httpClient,
        int equipmentId,
        int historyId)
    {
        return httpClient.DeleteAsync(
             $"api/EquipmentHistory/{equipmentId}/history/{historyId}",
            _cancellationToken);
    }

    public static Task<Stream> GetEquipmentFileStreamAsync(
        this HttpClient httpClient,
        DocumentType documentType,
        int approverId,
        string serialNumber,
        int top = 1,
        int skip = 0)
    {
        return httpClient.GetStreamAsync(
            $"api/Equipment/file/{documentType}?top={top}&skip={skip}&filter=ApproverId eq {approverId} and SerialNumber eq '{serialNumber}'",
            _cancellationToken);
    }

    public static Task<Stream> GetEquipmentFileStreamAsync(
        this HttpClient httpClient,
        DocumentType documentType,
        int approverId,
        string serialNumber,
        string equipmentTypeId,
        int top = 1,
        int skip = 0)
    {
        return httpClient.GetStreamAsync(
            $"api/Equipment/file/{documentType}?top={top}&skip={skip}&filter=ApproverId eq {approverId} and SerialNumber eq '{serialNumber}' and EquipmentTypeId eq '{equipmentTypeId}'",
            _cancellationToken);
    }

    public static Task<Stream> GetEquipmentFileStreamAsync(
        this HttpClient httpClient,
        DocumentType documentType,
        int top = 1,
        int skip = 0)
    {
        return httpClient.GetStreamAsync(
            $"api/Equipment/file/{documentType}?$orderby=Id%20desc&$top={top}&$skip={skip}",
            _cancellationToken);
    }

    public static Task<EquipmentHolderOutput?> CreateEquipmentHistoryAsOutputAsync(
        this HttpClient httpClient,
        int id,
        AssignEquipmentInput assignEquipmentInput)
    {
        return httpClient.PostAsJsonAsync<AssignEquipmentInput, EquipmentHolderOutput>(
            $"api/EquipmentHistory/{id}/assign",
            assignEquipmentInput,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateEquipmentHistoryAsync(
        this HttpClient httpClient,
        int id,
        AssignEquipmentInput assignEquipmentInput)
    {
        return httpClient.PostAsJsonAsync(
            $"api/EquipmentHistory/{id}/assign",
            assignEquipmentInput,
            _cancellationToken);
    }

    public static Task<EquipmentHolderOutput?> CreateEquipmentHistoryWithAsOutputReturnAsync(
        this HttpClient httpClient,
        int id,
        ReturnEquipmentInput returnEquipmentInput)
    {
        return httpClient.PostAsJsonAsync<ReturnEquipmentInput, EquipmentHolderOutput>(
            $"api/EquipmentHistory/{id}/return",
            returnEquipmentInput,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateEquipmentHistoryWithReturnAsync(
        this HttpClient httpClient,
        int id,
        ReturnEquipmentInput returnEquipmentInput)
    {
        return httpClient.PostAsJsonAsync(
            $"api/EquipmentHistory/{id}/return",
            returnEquipmentInput,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateEquipmentAssignAsync(
        this HttpClient httpClient,
        int id,
        AssignEquipmentInput assignEquipmentInput)
    {
        return httpClient.PostAsJsonAsync(
            $"api/Equipment/{id}/assign",
            assignEquipmentInput,
            _cancellationToken);
    }

    public static Task<PageableUserEquipmentOutput?> GetUserEquipmentAsync(
        this HttpClient httpClient,
        int id,
        string filter)
    {
        return httpClient.GetFromJsonAsync<PageableUserEquipmentOutput>(
            $"/api/users/{id}/equipment?{filter}",
            _cancellationToken);
    }
}
