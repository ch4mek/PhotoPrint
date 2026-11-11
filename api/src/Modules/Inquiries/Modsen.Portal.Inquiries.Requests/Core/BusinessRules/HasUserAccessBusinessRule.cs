using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Core.BusinessRules;

internal interface IHasUserAccessBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int inquiryId, AccessOptions accessOptions, CancellationToken cancellationToken);
    Task<bool> IsCorrespondsToBusiness(int? inquiryId, int userIdInquiryFor, AccessOptions accessOptions, CancellationToken cancellationToken);
}

internal class HasUserAccessBusinessRule : IHasUserAccessBusinessRule
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IHasUserAccessToInquiryBusinessRule _hasUserAccessToInquiryBusinessRule;
    private readonly IIsUserApproverBusinessRule _isUserApproverBusinessRule;
    private readonly IGetInquiriesCanBeApprovedAsSubstituteResolver _getInquiriesCanBeApprovedAsSubstituteResolver;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public HasUserAccessBusinessRule(IInquiriesDbContext inquiriesDbContext,
        IHasUserAccessToInquiryBusinessRule hasUserAccessToInquiryBusinessRule,
        IIsUserApproverBusinessRule isUserApproverBusinessRule,
        IGetInquiriesCanBeApprovedAsSubstituteResolver getInquiriesCanBeApprovedAsSubstituteResolver,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _hasUserAccessToInquiryBusinessRule = hasUserAccessToInquiryBusinessRule;
        _isUserApproverBusinessRule = isUserApproverBusinessRule;
        _getInquiriesCanBeApprovedAsSubstituteResolver = getInquiriesCanBeApprovedAsSubstituteResolver;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int inquiryId, AccessOptions accessOptions, CancellationToken cancellationToken)
    {
        var userIdInquiryFor = await _inquiriesDbContext.UserInquiries
            .AsNoTracking()
            .Where(x => x.Id == inquiryId)
            .Select(x => x.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userIdInquiryFor == 0)
        {
            throw new EntityNotFoundException(PortalMessages.UserInquiryNotFound);
        }

        return await IsCorrespondsToBusiness(inquiryId, userIdInquiryFor, accessOptions, cancellationToken);
    }
    
    public async Task<bool> IsCorrespondsToBusiness(int? inquiryId, int userIdInquiryFor, AccessOptions accessOptions,
        CancellationToken cancellationToken)
    {
        if (accessOptions.HasFlag(AccessOptions.Manager))
        {
            var hasAccess = await _hasUserAccessToInquiryBusinessRule.IsCorrespondsToBusiness(userIdInquiryFor,
                accessOptions, cancellationToken);

            if (hasAccess)
            {
                return true;
            }
        }

        if (inquiryId == null)
        {
            return false;
        }

        if (accessOptions.HasFlag(AccessOptions.Approver))
        {
            var hasAccess = await _isUserApproverBusinessRule
                .IsCorrespondsToBusiness(inquiryId!.Value, cancellationToken);

            if (hasAccess)
            {
                return true;
            }
        }

        if (accessOptions.HasFlag(AccessOptions.Substitute))
        {
            var hasAccess = (await _getInquiriesCanBeApprovedAsSubstituteResolver
                .ResolveAsync([inquiryId!.Value], cancellationToken))
                .Count != 0;

            if (hasAccess)
            {
                return true;
            }
        }

        if (accessOptions.HasFlag(AccessOptions.RequestViewer) &&
            _authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ViewAllRequests.Name))
        {
            return true;
        }

        return false;
    }
}
