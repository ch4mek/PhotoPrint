using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Merch.Requests.Kits.Core.BusinessRules;
using Modsen.Portal.Merch.Requests.Kits.Core.Calculators;
using Modsen.Portal.Merch.Requests.Kits.Core.Loaders;
using Modsen.Portal.Merch.Requests.Kits.Core.Resolvers;
using Modsen.Portal.Merch.Requests.Kits.Core.Validators;

namespace Modsen.Portal.Merch.Requests.Kits;

public sealed class KitsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ICreateOrUpdateKitValidator, CreateOrUpdateKitValidator>();

        services.AddScoped<IAllKitMinAmountsListedBusinessRule, AllKitMinAmountsListedBusinessRule>();
        services.AddScoped<IAreCompoundsValidBusinessRule, AreCompoundsValidBusinessRule>();
        services.AddScoped<IKitNameUniqueBusinessRule, KitNameUniqueBusinessRule>();

        services.AddScoped<ICompoundSizesInStockResolver, CompoundSizesInStockResolver>();
        services.AddScoped<IKitsWithCompoundsLoader, KitsWithCompoundsLoader>();

        services.AddScoped<INonClothingItemSizesPossibleAmountsCalculator, NonClothingItemSizesPossibleAmountsCalculator>();
        services.AddScoped<IClothingItemSizesPossibleAmountsCalculator, ClothingItemSizesPossibleAmountsCalculator>();
        services.AddScoped<ITotalPossibleKitsWithClothingCalculator, TotalPossibleKitsWithClothingCalculator>();

        services.AddScoped<IKitPossibleAmountCalculator, KitPossibleAmountCalculator>();

    }
}
