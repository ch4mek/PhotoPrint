using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;

internal interface IInquiryAcceptsComposer
{
    Task<List<UserInquiryAcceptEntity>> ComposeAsync(int userId, int templateId, CancellationToken cancellationToken);
}

internal sealed class InquiryAcceptsComposer : IInquiryAcceptsComposer
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IIdentityContract _identityContract;
    private readonly ITimeTrackContract _timeTrackContract;

    public InquiryAcceptsComposer(IInquiriesDbContext inquiriesDbContext, IIdentityContract identityContract,
        ITimeTrackContract timeTrackContract)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _identityContract = identityContract;
        _timeTrackContract = timeTrackContract;
    }

    public async Task<List<UserInquiryAcceptEntity>> ComposeAsync(int userId, int templateId,
        CancellationToken cancellationToken)
    {
        var templatePolicies = await _inquiriesDbContext.InquiryTemplateAcceptPolicies
            .Where(acceptPolicy => acceptPolicy.InquiryTemplateId == templateId)
            .OrderBy(acceptPolicy => acceptPolicy.OrderWeight)
                .ThenBy(acceptPolicy => acceptPolicy.Id)
            .Select(acceptPolicy => new
            {
                acceptPolicy.Id,
                acceptPolicy.Scope,
                acceptPolicy.UserId,
            })
            .ToListAsync(cancellationToken);

        var userPosition = await GetUserPositionAsync(userId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

        var availablePolicies = AcceptPolicyScopesForPosition.GetAvailable(userPosition);
        var availableTemplatePolicies = templatePolicies
            .Join(availablePolicies,
                x => x.Scope,
                y => y,
                (x, y) => x);

        var acceptPolicies = new List<UserInquiryAcceptEntity>();

        foreach (var templatePolicy in availableTemplatePolicies)
        {
            switch (templatePolicy.Scope)
            {
                case InquiryAcceptPolicyScopes.GroupManager:
                    {
                        await AddGroupManagerAcceptAsync(acceptPolicies, userId, cancellationToken);
                        break;
                    }

                case InquiryAcceptPolicyScopes.HeadOfDepartment:
                    {
                        await AddHeadOfDepartmentAcceptAsync(acceptPolicies, userId, cancellationToken);
                        break;
                    }

                case InquiryAcceptPolicyScopes.ProjectManager:
                    {
                        await AddProjectManagerAcceptsAsync(acceptPolicies, userId, cancellationToken);
                        break;
                    }

                case InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO:
                    {
                        if (userPosition.IsCto())
                        {
                            await AddCeoAcceptAsync(acceptPolicies, cancellationToken);
                            break;
                        }

                        await AddChiefAcceptAsync(acceptPolicies, userId, templatePolicy.Scope, cancellationToken);
                        break;
                    }

                case InquiryAcceptPolicyScopes.User:
                    {
                        await AddUserAcceptAsync(acceptPolicies, templatePolicy.UserId, cancellationToken);
                        break;
                    }
            }
        }

        return acceptPolicies;
    }

    private async Task AddGroupManagerAcceptAsync(List<UserInquiryAcceptEntity> acceptPolicies,
        int userId, CancellationToken cancellationToken)
    {
        var groupManagerId = await _identityContract.GetUserGroupManagerId(userId, cancellationToken);

        if (groupManagerId.HasValue)
        {
            acceptPolicies.Add(new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.Pending,
                AcceptUserId = groupManagerId.Value,
                AcceptUserPosition = PortalRoleNames.GroupManager,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager
            });
        }
    }

    private async Task AddHeadOfDepartmentAcceptAsync(List<UserInquiryAcceptEntity> acceptPolicies,
        int userId, CancellationToken cancellationToken)
    {
        var headId = await _identityContract.GetUserHeadId(userId, cancellationToken);

        if (headId.HasValue)
        {
            acceptPolicies.Add(new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.Pending,
                AcceptUserId = headId.Value,
                AcceptUserPosition = PortalRoleNames.HeadOfDepartment,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment
            });
        }
        else
        {
            await AddChiefAcceptAsync(acceptPolicies, userId, InquiryAcceptPolicyScopes.HeadOfDepartment,
                cancellationToken);
        }
    }

    private async Task AddProjectManagerAcceptsAsync(List<UserInquiryAcceptEntity> acceptPolicies,
        int userId, CancellationToken cancellationToken)
    {
        
        var projectManagerIds = await _timeTrackContract.GetProjectManagerIdsByUserIdAsync(userId,
            cancellationToken);

        var projectManagers = await _inquiriesDbContext.InquiriesUsers
            .Where(x => projectManagerIds.Contains(x.UserId))
            .Where(x => x.UserId != userId)
            .Select(x => new { x.UserId, x.Position })
            .ToListAsync(cancellationToken);

        foreach (var projectManager in projectManagers)
        {
            acceptPolicies.Add(new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.Pending,
                AcceptUserId = projectManager.UserId,
                AcceptUserPosition = projectManager.Position,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager
            });
        }      
    }

    private async Task AddChiefAcceptAsync(List<UserInquiryAcceptEntity> acceptPolicies, int userId,
        InquiryAcceptPolicyScopes scope, CancellationToken cancellationToken)
    {
        var chiefId = await _identityContract.GetUserChiefIdAsync(userId, cancellationToken);

        if (chiefId.HasValue)
        {
            var chiefPosition = await GetUserPositionAsync(chiefId.Value, cancellationToken);

            acceptPolicies.Add(new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.Pending,
                AcceptUserId = chiefId.Value,
                AcceptUserPosition = chiefPosition,
                AcceptPolicyScope = scope
            });
        }
    }

    private async Task AddCeoAcceptAsync(List<UserInquiryAcceptEntity> acceptPolicies,
        CancellationToken cancellationToken)
    {
        var ceoId = await _inquiriesDbContext.InquiriesUsers
            .Where(x => x.Position == PortalRoleNames.Ceo)
            .Where(x => x.IsActive)
            .Select(x => x.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (ceoId != 0)
        {
            acceptPolicies.Add(new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.Pending,
                AcceptUserId = ceoId,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO,
                AcceptUserPosition = PortalRoleNames.Ceo
            });
        }
    }

    private async Task AddUserAcceptAsync(List<UserInquiryAcceptEntity> acceptPolicies, int? policyUserId,
        CancellationToken cancellationToken)
    {
        if (policyUserId.HasValue)
        {
            var approverPosition = await GetUserPositionAsync(policyUserId!.Value, cancellationToken)
            ?? throw new EntityNotFoundException("User for approve not found");

            acceptPolicies.Add(new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.Pending,
                AcceptUserId = policyUserId!.Value,
                AcceptUserPosition = approverPosition,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.User
            });
        }
    }

    private Task<string?> GetUserPositionAsync(int userId, CancellationToken cancellationToken)
    {
        return _inquiriesDbContext.InquiriesUsers
            .Where(x => x.UserId == userId)
            .Select(x => x.Position)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
