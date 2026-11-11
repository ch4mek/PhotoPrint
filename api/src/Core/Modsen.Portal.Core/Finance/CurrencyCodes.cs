namespace Modsen.Portal.Core.Finance;

public static class CurrencyCodes
{
    public const string BelarusianRuble = "BYN";
    public const string USDollar = "USD";
    public const string Euro = "EUR";
    public const string RussianRuble = "RUB";
    public const string ChineseYuan = "CNY";
    public const string PoundSterling = "GBP";
    public const string SwissFranc = "CHF";
    public const string CanadianDollar = "CAD";
    public const string AustralianDollar = "AUD";
    public const string NewZealandDollar = "NZD";
    public const string JapaneseYen = "JPY";
    public const string IndianRupee = "INR";
    public const string KazakhstaniTenge = "KZT";
    public const string UzbekistaniSom = "UZS";
    public const string KyrgyzstaniSom = "KGS";
    public const string IranianRial = "IRR";
    public const string VietnameseDong = "VND";
    public const string PakistaniRupee = "PKR";
    public const string PolishZloty = "PLN";
    public const string CzechKoruna = "CZK";
    public const string SwedishKrona = "SEK";
    public const string NorwegianKrone = "NOK";
    public const string DanishKrone = "DKK";
    public const string IcelandicKrona = "ISK";
    public const string HungarianForint = "HUF";
    public const string UkrainianHryvnia = "UAH";
    public const string TurkishLira = "TRY";
    public const string IsraeliNewShekel = "ILS";
    public const string AzerbaijaniManat = "AZN";
    public const string ArmenianDram = "AMD";
    public const string SouthAfricanRand = "ZAR";
    public const string TurkmenistaniManat = "TMT";
    public const string EgyptianPound = "EGP";
    public const string UAEDirham = "AED";
    public const string BrazilianReal = "BRL";

    public static IReadOnlyCollection<string> ToList()
    {
        return [
            BelarusianRuble,
            USDollar,
            Euro,
            RussianRuble,
            ChineseYuan,
            PoundSterling,
            SwissFranc,
            CanadianDollar,
            AustralianDollar,
            NewZealandDollar,
            JapaneseYen,
            IndianRupee,
            KazakhstaniTenge,
            UzbekistaniSom,
            KyrgyzstaniSom,
            IranianRial,
            VietnameseDong,
            PakistaniRupee,
            PolishZloty,
            CzechKoruna,
            SwedishKrona,
            NorwegianKrone,
            DanishKrone,
            IcelandicKrona,
            HungarianForint,
            UkrainianHryvnia,
            TurkishLira,
            IsraeliNewShekel,
            AzerbaijaniManat,
            ArmenianDram,
            SouthAfricanRand,
            TurkmenistaniManat,
            EgyptianPound,
            UAEDirham,
            BrazilianReal];
    }
}
