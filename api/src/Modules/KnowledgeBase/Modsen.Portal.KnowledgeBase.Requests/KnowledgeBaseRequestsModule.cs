using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups;
using Modsen.Portal.KnowledgeBase.Requests.Articles;
using Modsen.Portal.KnowledgeBase.Requests.Consumers;
using Modsen.Portal.KnowledgeBase.Requests.Core;
using Modsen.Portal.KnowledgeBase.Requests.Core.Calculators;
using Modsen.Portal.KnowledgeBase.Requests.Core.DbQueries;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory;
using Modsen.Portal.KnowledgeBase.Requests.Discounts;

namespace Modsen.Portal.KnowledgeBase.Requests;

public sealed class KnowledgeBaseRequestsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        AddModule<DiscountCategoriesModule>();
        AddModule<DiscountsModule>();
        AddModule<GroupsModule>();
        AddModule<ArticlesModule>();

        base.Load(services);

        services.AddScoped<IUrlsFromTextExtractor, UrlsFromTextExtractor>();
        services.AddScoped<IMaxOrderWeightCalculator, MaxOrderWeightCalculator>();
        services.AddScoped<ICreateOrUpdateUserDbQuery, CreateOrUpdateUserDbQuery>();
    }


    public override IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        yield return new UserCreatedEventConsumer.RegistrationDefinition();
        yield return new UserSyncEventConsumer.RegistrationDefinition();
        yield return new UserUpdatedEventConsumer.RegistrationDefinition();
    }
}
