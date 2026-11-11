using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Seeds;

internal class CurrenciesSeed : ISeedEntitiesProvider<FinanceDbContext>
{
    private readonly FinanceDbContext _dbContext;

    public CurrenciesSeed(FinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        foreach (var currency in GetCurrencies())
        {
            if (!_dbContext.Currencies.Any(x => x.Id == currency.Id))
            {
                _dbContext.Currencies.Add(currency);
            }
        }

        _dbContext.SaveChanges();
    }

    private static IEnumerable<CurrencyEntity> GetCurrencies()
    {
        yield return CreateCurrency(CurrencyCodes.USDollar, "Доллар США", CurrencyCodes.USDollar, "$");
        yield return CreateCurrency(CurrencyCodes.Euro, "Евро", CurrencyCodes.Euro, "€");
        yield return CreateCurrency(CurrencyCodes.BelarusianRuble, "Белорусский рубль", CurrencyCodes.BelarusianRuble, null);
        yield return CreateCurrency(CurrencyCodes.PolishZloty, "Злотый", CurrencyCodes.PolishZloty, "zł");
        yield return CreateCurrency(CurrencyCodes.RussianRuble, "Российский рубль", CurrencyCodes.RussianRuble, "₽");
        yield return CreateCurrency(CurrencyCodes.ChineseYuan, "Китайский юань", CurrencyCodes.ChineseYuan, "¥");
        yield return CreateCurrency(CurrencyCodes.PoundSterling, "Фунт стерлингов", CurrencyCodes.PoundSterling, "£");
        yield return CreateCurrency(CurrencyCodes.SwissFranc, "Швейцарский франк", CurrencyCodes.SwissFranc, null);
        yield return CreateCurrency(CurrencyCodes.CanadianDollar, "Канадский доллар", CurrencyCodes.CanadianDollar, "C$");
        yield return CreateCurrency(CurrencyCodes.AustralianDollar, "Австралийский доллар", CurrencyCodes.AustralianDollar, "A$");
        yield return CreateCurrency(CurrencyCodes.NewZealandDollar, "Новозеландский доллар", CurrencyCodes.NewZealandDollar, "$");
        yield return CreateCurrency(CurrencyCodes.JapaneseYen, "Японская йена", CurrencyCodes.JapaneseYen, "¥");
        yield return CreateCurrency(CurrencyCodes.IndianRupee, "Индийская рупия", CurrencyCodes.IndianRupee, "₹");
        yield return CreateCurrency(CurrencyCodes.KazakhstaniTenge, "Казахстанский тенге", CurrencyCodes.KazakhstaniTenge, "₸");
        yield return CreateCurrency(CurrencyCodes.UzbekistaniSom, "Узбекский сум", CurrencyCodes.UzbekistaniSom, null);
        yield return CreateCurrency(CurrencyCodes.KyrgyzstaniSom, "Киргизский сом", CurrencyCodes.KyrgyzstaniSom, null);
        yield return CreateCurrency(CurrencyCodes.IranianRial, "Иранский риал", CurrencyCodes.IranianRial, null);
        yield return CreateCurrency(CurrencyCodes.VietnameseDong, "Вьетнамский донг", CurrencyCodes.VietnameseDong, "₫");
        yield return CreateCurrency(CurrencyCodes.PakistaniRupee, "Пакистанская рупия", CurrencyCodes.PakistaniRupee, "₨");
        yield return CreateCurrency(CurrencyCodes.CzechKoruna, "Чешская крона", CurrencyCodes.CzechKoruna, null);
        yield return CreateCurrency(CurrencyCodes.SwedishKrona, "Шведская крона", CurrencyCodes.SwedishKrona, null);
        yield return CreateCurrency(CurrencyCodes.NorwegianKrone, "Норвежская крона", CurrencyCodes.NorwegianKrone, null);
        yield return CreateCurrency(CurrencyCodes.DanishKrone, "Датская крона", CurrencyCodes.DanishKrone, null);
        yield return CreateCurrency(CurrencyCodes.IcelandicKrona, "Исландская крона", CurrencyCodes.IcelandicKrona, null);
        yield return CreateCurrency(CurrencyCodes.HungarianForint, "Венгерский форинт", CurrencyCodes.HungarianForint, null);
        yield return CreateCurrency(CurrencyCodes.UkrainianHryvnia, "Украинская гривна", CurrencyCodes.UkrainianHryvnia, "₴");
        yield return CreateCurrency(CurrencyCodes.TurkishLira, "Турецкая лира", CurrencyCodes.TurkishLira, "₺");
        yield return CreateCurrency(CurrencyCodes.IsraeliNewShekel, "Новый израильский шекель", CurrencyCodes.IsraeliNewShekel, "₪");
        yield return CreateCurrency(CurrencyCodes.AzerbaijaniManat, "Азербайджанский манат", CurrencyCodes.AzerbaijaniManat, "₼");
        yield return CreateCurrency(CurrencyCodes.ArmenianDram, "Армянский драм", CurrencyCodes.ArmenianDram, "֏");
        yield return CreateCurrency(CurrencyCodes.SouthAfricanRand, "Южноафриканский рэнд", CurrencyCodes.SouthAfricanRand, "R");
        yield return CreateCurrency(CurrencyCodes.TurkmenistaniManat, "Туркменский манат", CurrencyCodes.TurkmenistaniManat, null);
        yield return CreateCurrency(CurrencyCodes.EgyptianPound, "Египетский фунт", CurrencyCodes.EgyptianPound, null);
        yield return CreateCurrency(CurrencyCodes.UAEDirham, "Дирхам ОАЭ", CurrencyCodes.UAEDirham, null);
        yield return CreateCurrency(CurrencyCodes.BrazilianReal, "Бразильский реал", CurrencyCodes.BrazilianReal, "R$");
    }

    private static CurrencyEntity CreateCurrency(string id, string name, string code, string? symbol)
    {
        return new CurrencyEntity { Id = id, Name = name, CurrencyCode = code, CurrencySymbol = symbol };
    }
}
