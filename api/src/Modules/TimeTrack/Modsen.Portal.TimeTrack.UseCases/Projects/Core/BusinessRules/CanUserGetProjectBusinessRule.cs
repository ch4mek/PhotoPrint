using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;

internal interface ICanUserGetProjectBusinessRule
{
    Task<BusinessRuleResult> IsCorrespondsToBusiness(int projectId, CancellationToken cancellationToken);
}

internal class CanUserGetProjectBusinessRule : IBusinessRuleWithFailReason<int>, ICanUserGetProjectBusinessRule
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CanUserGetProjectBusinessRule(
        ITimeTrackDbContext timeTrackDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(int projectId, CancellationToken cancellationToken)
    {
        var projectInfo = await _timeTrackDbContext.Projects
            .Where(project => project.Id == projectId)
            .Select(x => new
            {
                x.IsPublic,
                x.CreatedById,
                ProjectUsersDepartmentIds = x.ProjectUsers
                    .Select(y => y.User!.DepartmentId).ToList(),
                ResourceGroupIds = x.ProjectUsers
                    .Where(projectUser => projectUser.User!.ResourceGroupId != null)
                    .Select(y => y.User!.ResourceGroupId!.Value).Distinct().ToList()
            })
            .AsSplitQuery()
            .FirstAsync(cancellationToken);

        return _authenticatedUserContext.GetRoles() switch
        {
            var roles when roles.IsCto() || roles.IsCeo()  => BusinessRuleResult.Success(),
            var roles when roles.IsHeadOfDepartment() => CanHeadOfDepartmentGetProject(
                projectInfo.CreatedById,
                projectInfo.ProjectUsersDepartmentIds!,
                projectInfo.IsPublic),
            var roles when roles.IsGroupManager() => CanGroupManagerGetProject(
                projectInfo.IsPublic,
                projectInfo.CreatedById,
                projectInfo.ResourceGroupIds),
            _ => BusinessRuleResult.Fail(PortalMessages.UnsupportedRoleTypes)
        };
    }

    private BusinessRuleResult CanHeadOfDepartmentGetProject(
        int? createdById,
        List<string> projectUsersDepartmentIds,
        bool isPublic)
    {
        var isCreator = createdById == _authenticatedUserContext.UserId;
        var isAnyProjectUserFromCurrentUserDepartment = projectUsersDepartmentIds.Count != 0 &&
            projectUsersDepartmentIds.Any(x => x == _authenticatedUserContext.DepartmentId);

        if (!(isCreator || isPublic || isAnyProjectUserFromCurrentUserDepartment))
        {
            return BusinessRuleResult.Fail(PortalMessages.HeadOfDepartmentDoesNotHaveAccessToTheProject);
        }

        return BusinessRuleResult.Success();
    }

    private BusinessRuleResult CanGroupManagerGetProject(
        bool isPublic,
        int? createdById,
        List<int> resourceGroupIds)
    {
        var isCreator = createdById == _authenticatedUserContext.UserId;

        var isResourceGroupExistingAtProject = resourceGroupIds.Exists(id => id == _authenticatedUserContext.ResourceGroupId);

        if (!(isCreator || isPublic || isResourceGroupExistingAtProject))
        {
            return BusinessRuleResult.Fail(PortalMessages.GroupManagerDoesNotHaveAccessToProjectsOfUserFromAnotherResourceGroup);
        }

        return BusinessRuleResult.Success();
    }
}
