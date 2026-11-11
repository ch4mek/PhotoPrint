using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments.Core.BusinessRules;

internal interface ICanUserAccessResourcesBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(CanUserAccessResourcesBusinessRule.Request model,
        CancellationToken cancellationToken);
}

internal class CanUserAccessResourcesBusinessRule : ICanUserAccessResourcesBusinessRule
{
    private readonly ITimeTrackContract _timeTrackContract;

    public CanUserAccessResourcesBusinessRule(ITimeTrackContract timeTrackContract)
    {
        _timeTrackContract = timeTrackContract;
    }

    public async Task<bool> IsCorrespondsToBusiness(
        Request? model,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);
        return model.ResourceType switch
        {
            PermissionResourceType.Project => await CanUserAccessProjectsAsync(model.Userid, model.Resources,
                cancellationToken),

            _ => false
        };
    }

    private async Task<bool> CanUserAccessProjectsAsync(int userId, IReadOnlyDictionary<string, string> resources, CancellationToken cancellationToken)
    {
        var projectIds = resources.Select(resource =>
        {
            var canParse = int.TryParse(resource.Key, out var projectId);

            return canParse
                ? projectId
                : throw new ArgumentException(PortalMessages.CannotParseValue);

        }).ToList();

        var projectModels = await _timeTrackContract
            .GetListOfProjectsByIds(projectIds, cancellationToken);

        var allProjectExists = Modsen.Portal.Core.Utils.Extensions.CollectionExtensions
            .AreEquivalentIgnoreOrdering(projectModels.Select(x => x.Id), projectIds);

        if (!allProjectExists)
        {
            throw new EntityNotFoundException(PortalMessages.ProjectsNotExist);
        }

        var doesUserHaveAccessToAllProjects = projectModels.All(project =>
        {
            var isUserAssignedToProject = project.ProjectUsers.Any(pu => pu.UserId == userId);

            return (isUserAssignedToProject || project.IsPublic) && project.IsActive;
        });

        return doesUserHaveAccessToAllProjects;
    }

    internal record Request(
        int Userid,
        PermissionResourceType ResourceType,
        IReadOnlyDictionary<string, string> Resources);
}
