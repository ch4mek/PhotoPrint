using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests;

internal sealed class GetUserSoftSkillsRequest
    : IQueryHandler<GetUserSoftSkillsRequest.Query, PageableListOfItems<OdpUserSoftSkill>>
{
    private readonly IIsTeamBoardUserExistWithAccessControlDbQuery _isUserExistByUserIdDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetUserSoftSkillsRequest(IAuthenticatedUserContext authenticatedUserContext,
        IIsTeamBoardUserExistWithAccessControlDbQuery isUserExistByUserIdDbQuery,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _isUserExistByUserIdDbQuery = isUserExistByUserIdDbQuery;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<PageableListOfItems<OdpUserSoftSkill>> Handle(Query request, CancellationToken cancellationToken)
    {
        var isEmployeeOrSysAdmin = _authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsSystemAdministrator();

        if (isEmployeeOrSysAdmin && request.UserId != _authenticatedUserContext.UserId)
        {
            throw new ActionNotAllowedException("You are not allowed to get another user soft-skills.");
        }

        var isTeamBoardUserExisting = await _isUserExistByUserIdDbQuery
            .ExecuteAsync(request.UserId, cancellationToken: cancellationToken);

        if (!isTeamBoardUserExisting)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        var pageableSoftSkills = await GetSoftSkillsAsync(request.UserId, request.QueryOptions, cancellationToken);

        return pageableSoftSkills;
    }

    private async Task<PageableListOfItems<OdpUserSoftSkill>> GetSoftSkillsAsync(int userId,
        ODataQueryOptions<OdpUserSoftSkill> queryOptions,
        CancellationToken cancellationToken)
    {
        var query = _teamBoardDbContext.SoftSkills
            .AsNoTracking()
            .Where(softSkill => softSkill.IsActive)
            .Select(softSkill => new OdpUserSoftSkill
            {
                Id = softSkill.Id,
                SoftSkillName = softSkill.Name,
                Snapshots = _teamBoardDbContext.UserSoftSkillsSnapshots
                    .Where(snapshot => snapshot.UserId == userId)
                    .Where(snapshot => snapshot.SoftSkills.Any(us => us.SoftSkillId == softSkill.Id))
                    .Select(snapshot => new OdpUserSnapshot
                    {
                        SnapshotDate = snapshot.SnapshotDate,
                        SnapshotId = snapshot.Id,
                        SoftSkillSeniority = snapshot.SoftSkills
                            .FirstOrDefault(skill => skill.SoftSkillId == softSkill.Id)!
                            .SeniorityLevel!.ExternalName,
                        SoftSkillSeniorityOrderWeight = snapshot.SoftSkills
                            .FirstOrDefault(skill => skill.SoftSkillId == softSkill.Id)!
                            .SeniorityLevel!.OrderWeight
                    }).ToList()

            });

        var result = await queryOptions.ApplyTo(query)
            .Cast<OdpUserSoftSkill>()
            .ToListAsync(cancellationToken);

        var totalCount = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .Cast<OdpUserSoftSkill>()
            .CountAsync(cancellationToken);

        return result.ToPageable(queryOptions, totalCount);
    }

    internal sealed record Query(int UserId, ODataQueryOptions<OdpUserSoftSkill> QueryOptions)
        : IQuery<PageableListOfItems<OdpUserSoftSkill>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
