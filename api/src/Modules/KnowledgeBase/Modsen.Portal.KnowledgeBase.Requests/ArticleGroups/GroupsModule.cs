using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Core;

namespace Modsen.Portal.KnowledgeBase.Requests.ArticleGroups;

internal sealed class GroupsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IIsGroupWithSameNameExistBusinessRule, IsGroupWithSameNameExistBusinessRule>();
        services.AddScoped<IGroupLoader, GroupLoader>();
    }
}
