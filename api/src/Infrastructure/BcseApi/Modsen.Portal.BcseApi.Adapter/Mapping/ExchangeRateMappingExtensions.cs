using System.Globalization;
using System.Text.RegularExpressions;
using Modsen.Portal.BcseApi.Adapter.ApiModels;
using Modsen.Portal.BcseApi.Contract.Models;
using Modsen.Portal.Core.Exceptions;

namespace Modsen.Portal.BcseApi.Adapter.Mapping;

internal static class ExchangeRateMappingExtensions
{
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(100);

    internal static ExchangeRateModel Map(this ExchangeRateApiModel model)
    {
        var match = Regex.Match(model.RateCoef, @"^(\d+)\s+([A-Z]+)$", RegexOptions.None, RegexTimeout);

        if (!match.Success)
        {
            throw new AppException($"Error during parsing rateCoef from Bcse API response. RateCoef from Bcse API response: {model.RateCoef}");
        }

        return new ExchangeRateModel()
        {
            CurrencyName = model.Currency,
            CurrencyCode = match.Groups[2].Value,
            RateCoef = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
            RateNB = model.RateNB
        };
    }
}
