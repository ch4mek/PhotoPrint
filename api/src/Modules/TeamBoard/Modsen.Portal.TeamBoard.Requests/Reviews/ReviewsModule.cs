using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.AuthenticationFilters;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.ApiHandlers;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Core;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.Reviews;

public sealed class ReviewsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ICreatePerformanceReviewComposer, CreatePerformanceReviewComposer>();
        services.AddScoped<IUpdatePerformanceReviewComposer, UpdatePerformanceReviewComposer>();
        services.AddScoped<IGetPerformanceReviewByConversationIdDbQuery, GetUserPerformanceReviewByConversationIdDbQuery>();
        services.AddScoped<IIsTheSamePerformanceReviewDbQuery, IsTheSamePerformanceReviewDbQuery>();
        services.AddScoped<IConversationLoader, ConversationLoader>();
        services.AddScoped<IIsConversationExistingDbQuery, IsConversationExistingDbQuery>();
        services.AddScoped<IIsPerformanceReviewForTodayExistingBusinessRule, IsPerformanceReviewForTodayExistingBusinessRule>();
        services.AddScoped<IIsUserHavingConversationBusinessRule, IsUserHavingConversationBusinessRule>();
        services.AddScoped<IIsUserHasAccessToMutateConversation, IsUserHasAccessToMutateConversation>();
        services.AddScoped<IConversationsAuthenticationFilter, ConversationsAuthenticationFilter>();

        services.AddScoped<IDevelopmentPlansApi, DevelopmentPlansApi>();
        services.AddScoped<IUpdateDevelopmentPlanListWithCreatingApiHandler, UpdateDevelopmentPlanListWithCreatingApiHandler>();
        services.AddScoped<IDevelopmentPlanLoader, DevelopmentPlanLoader>();

        services.AddScoped<IPerformanceReviewDateTypeCalculator, PerformanceReviewDateTypeCalculator>();
    }
}
