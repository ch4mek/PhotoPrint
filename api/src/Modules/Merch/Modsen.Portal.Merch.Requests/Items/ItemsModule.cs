using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Merch.Requests.Items.Core.BusinessRules;
using Modsen.Portal.Merch.Requests.Items.Core.Composers;
using Modsen.Portal.Merch.Requests.Items.Core.DbQueries;
using Modsen.Portal.Merch.Requests.Items.Core.Loaders;
using Modsen.Portal.Merch.Requests.Items.Core.Validators;

namespace Modsen.Portal.Merch.Requests.Items;

public sealed class ItemsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IAreAllSizesListedBusinessRule, AreAllSizesListedBusinessRule>();
        services.AddScoped<IAreAllStoragesListedBusinessRule, AreAllStoragesListedBusinessRule>();
        services.AddScoped<IIsItemNameUniqueBusinessRule, IsItemNameUniqueBusinessRule>();

        services.AddScoped<IItemSizeMinAmountComposer, ItemSizeMinAmountComposer>();

        services.AddScoped<IGetItemSizeMinAmountsDbQuery, GetItemSizeMinAmountsDbQuery>();
        services.AddScoped<IGetMerchItemByIdDbQuery, GetMerchItemByIdDbQuery>();
        services.AddScoped<IGetAvailableItemsDbQuery, GetAvailableItemsDbQuery>();

        services.AddScoped<IItemSizeMinAmountValidator, ItemSizeMinAmountValidator>();

        services.AddScoped<IItemLookupLoader, ItemLookupLoader>();
    }
}
