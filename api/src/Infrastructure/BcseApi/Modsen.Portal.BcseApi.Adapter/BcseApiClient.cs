using System.Globalization;
using System.Net.Http.Json;
using Modsen.Portal.BcseApi.Adapter.ApiModels;
using Modsen.Portal.BcseApi.Adapter.Mapping;
using Modsen.Portal.BcseApi.Contract;
using Modsen.Portal.BcseApi.Contract.Models;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.BcseApi.Adapter;

public sealed class BcseApiClient : IBcseApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BcseApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public Task<ExchangeRatesByDateModel> GetCurrentExchangeRates(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.ToLocal(LocationType.Minsk);

        return GetExchangeRates(DateOnly.FromDateTime(today), cancellationToken);
    }

    public Task<ExchangeRatesByDateModel> GetExchangeRatesByDate(DateOnly targetDate, CancellationToken cancellationToken)
    {
        return GetExchangeRates(targetDate, cancellationToken);
    }

    public async Task<List<ExchangeRatesByDateModel>> GetExchangeRatesByDates(IReadOnlyCollection<DateOnly> dates, CancellationToken cancellationToken)
    {
        if (dates is null)
        {
            return [];
        }

        var exchangeRatesByDate = new List<ExchangeRatesByDateModel>();

        foreach (var date in dates)
        {
            var exchangeRates = await GetExchangeRates(date, cancellationToken);

            exchangeRatesByDate.Add(exchangeRates);
        }

        return exchangeRatesByDate;
    }

    private async Task<ExchangeRatesByDateModel> GetExchangeRates(DateOnly targetDate, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(BcseApiConstants.BcseApiHttpClientName);

        var formattedDate = targetDate.ToString(BcseApiConstants.DateFormatInRequest, CultureInfo.InvariantCulture);
        var url = new Uri($"?sDate={formattedDate}", UriKind.Relative);

        var response = await client.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new AppException($"HTTP Error during getting exchange rates from Bcse API. Reason: {response.ReasonPhrase}");
        }

        ExchangeRateApiResponse? result;

        try
        {
            result = await response.Content.ReadFromJsonAsync<ExchangeRateApiResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new AppException(ex.Message);
        }

        if (result is null)
        {
            return new ExchangeRatesByDateModel
            {
                Date = targetDate,
                ExchangeRates = []
            };
        }

        var exchangeRates = new List<ExchangeRateModel>();

        foreach (var rate in result.Rates)
        {
            exchangeRates.Add(rate.Map());
        }

        var exchangeRatesByDateModel = new ExchangeRatesByDateModel
        {
            Date = targetDate,
            ExchangeRates = exchangeRates
        };

        return exchangeRatesByDateModel;
    }
}
