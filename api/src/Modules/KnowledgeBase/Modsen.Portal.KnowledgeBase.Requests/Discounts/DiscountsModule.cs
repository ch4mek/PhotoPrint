using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.BusinessRules;

namespace Modsen.Portal.KnowledgeBase.Requests.Discounts;

internal sealed class DiscountsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IDiscountLoader, DiscountLoader>();
        services.AddScoped<IHasDiscountCategoryAllDiscountTagsBusinessRule, HasDiscountCategoryAllDiscountTagsBusinessRule>();
        services.AddScoped<IImageFileIdValidator, ImageFileIdValidator>();
    }
}
