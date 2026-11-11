using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Core;

namespace Modsen.Portal.KnowledgeBase.Requests.Articles;

internal sealed class ArticlesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IIsArticleWithSameNameExistBusinessRule, IsArticleWithSameNameExistBusinessRule>();
        services.AddScoped<IArticleLoader, ArticleLoader>();
    }
}
