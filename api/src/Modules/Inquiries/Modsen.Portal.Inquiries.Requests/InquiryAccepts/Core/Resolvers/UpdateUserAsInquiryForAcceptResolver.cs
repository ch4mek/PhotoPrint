using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

internal interface IUpdateUserAsInquiryForAcceptResolver
{
    Task ResolveAsync(UpdateUserAcceptResolverRequest request, CancellationToken cancellationToken);
}

internal class UpdateUserAsInquiryForAcceptResolver : IUpdateUserAsInquiryForAcceptResolver
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IUpdateUserInfoInquiryAcceptsResolver _updateOpenInquiriesAcceptsResolver;

    public UpdateUserAsInquiryForAcceptResolver(IInquiriesDbContext inquiriesDbContext,
        IUpdateUserInfoInquiryAcceptsResolver updateOpenInquiriesAcceptsResolver)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _updateOpenInquiriesAcceptsResolver = updateOpenInquiriesAcceptsResolver;
    }

    public async Task ResolveAsync(UpdateUserAcceptResolverRequest request, CancellationToken cancellationToken)
    {
        var isSameDepartment = request.User.DepartmentId == request.PrevDepartmentId;
        var isSameGroup = request.User.ResourceGroupId == request.PrevResourceGroupId;
        var isEquivalentPosition = (request.User.Position.IsEmployee() || request.User.Position.IsSystemAdministrator())
            && (request.PrevPosition.IsEmployee() || request.PrevPosition.IsSystemAdministrator());

        if (isSameDepartment && isSameGroup && isEquivalentPosition)
        {
            return;
        }

        var openUserInquiries = await _inquiriesDbContext.UserInquiries
            .Where(x => x.UserId == request.User.UserId)
            .Where(x => x.Status == UserInquiryStatus.Open)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        await _updateOpenInquiriesAcceptsResolver.ResolveAsync(openUserInquiries, cancellationToken);
    }
}
