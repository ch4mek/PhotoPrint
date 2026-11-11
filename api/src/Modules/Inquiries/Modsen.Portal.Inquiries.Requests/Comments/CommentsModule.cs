using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Inquiries.Requests.Comments.Core;
using Modsen.Portal.Inquiries.Requests.Comments.Requests.Queries;

namespace Modsen.Portal.Inquiries.Requests.Comments;

internal sealed class CommentsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IGetInquiryCommentByIdDbQuery, GetInquiryCommentByIdDbQuery>();
        services.AddScoped<IInquiryCommentsLoader, InquiryCommentsLoader>();
    }
}
