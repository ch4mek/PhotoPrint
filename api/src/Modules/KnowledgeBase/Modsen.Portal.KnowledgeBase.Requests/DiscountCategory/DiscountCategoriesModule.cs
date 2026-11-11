using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core.BusinessRules;

namespace Modsen.Portal.KnowledgeBase.Requests.DiscountCategory;

internal sealed class DiscountCategoriesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IDiscountCategoryLoader, DiscountCategoryLoader>();
        services.AddScoped<IHasTagAnyDiscountsInCategory, HasTagAnyDiscountsInCategory>();
    }
}
