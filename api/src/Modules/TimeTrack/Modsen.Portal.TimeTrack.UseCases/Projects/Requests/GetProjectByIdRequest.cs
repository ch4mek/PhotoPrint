using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetProjectByIdRequest : IQueryHandler<GetProjectByIdRequest.Query, ProjectWithManagerDto>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly ICanUserGetProjectBusinessRule _canUserGetProjectBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetProjectByIdRequest(
        ITimeTrackDbContext timeTrackDbContext,
        ICanUserGetProjectBusinessRule canUserGetProjectBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _canUserGetProjectBusinessRule = canUserGetProjectBusinessRule;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<ProjectWithManagerDto> Handle(
        Query request,
        CancellationToken cancellationToken)
    {
        var project = await _timeTrackDbContext.Projects
            .Where(project => project.Id == request.Id)
            .Select(project => new ProjectWithManagerDto
            {
                Id = project.Id,
                Name = project.Name,
                Color = project.Color!,
                ClientId = project.ClientId,
                IsBillable = project.IsBillable,
                IsSelfCreatedProject = _authenticatedUserContext.UserId == project.CreatedById,
                IsActive = project.IsActive,
                IsPublic = project.IsPublic,
                ManagerId = project.ManagerUserId,
                ManagerFullName = project.ManagerUser == null
                    ? null
                    : project.ManagerUser.LastName + " " + project.ManagerUser.FirstName
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ProjectNotFound);

        var businessRuleResult = await _canUserGetProjectBusinessRule.IsCorrespondsToBusiness(request.Id, cancellationToken);

        if (!businessRuleResult.IsSuccess)
        {
            throw new ActionNotAllowedException(businessRuleResult.Reason!);
        }

        return project;
    }

    internal sealed record Query(int Id) : IQuery<ProjectWithManagerDto>, ITimeTrackRequest;
}
