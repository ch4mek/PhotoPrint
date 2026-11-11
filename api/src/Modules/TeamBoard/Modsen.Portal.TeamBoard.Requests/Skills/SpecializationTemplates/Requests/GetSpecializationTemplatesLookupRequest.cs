using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests;

internal sealed class GetSpecializationTemplatesLookupRequest
    : IQueryHandler<GetSpecializationTemplatesLookupRequest.Query, List<SpecializationTemplateDto>>
{
    private readonly IIsTeamBoardUserExistWithAccessControlDbQuery _isTeamBoardUserExistingWithAccessControlDbQuery;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetSpecializationTemplatesLookupRequest(
        IIsTeamBoardUserExistWithAccessControlDbQuery isTeamBoardUserExistingWithAccessControlDbQuery,
        ITeamBoardDbContext teamBoardDbContext)
    {

        _isTeamBoardUserExistingWithAccessControlDbQuery = isTeamBoardUserExistingWithAccessControlDbQuery;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<List<SpecializationTemplateDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (!await _isTeamBoardUserExistingWithAccessControlDbQuery.ExecuteAsync(request.UserId, cancellationToken: cancellationToken))
        {
            throw new EntityNotFoundException($"There is no user with id {request.UserId}");
        }

        var areSpecializationsAssigned = request.AreAssgined.HasValue && !request.AreAssgined.Value;

        var specializationTemplatesResult = areSpecializationsAssigned
            ? await GetSpecializationTemplatesUserDoesNotHave(request.UserId, cancellationToken)
            : await GetSpecializationTemplateUserHas(request.UserId, cancellationToken);

        return specializationTemplatesResult;
    }

    private async Task<List<SpecializationTemplateDto>> GetSpecializationTemplatesUserDoesNotHave(int userId,
        CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers
            .FirstAsync(usr => usr.UserId == userId, cancellationToken);

        return await _teamBoardDbContext.SpecializationTemplates
            .Where(specialization => specialization.UserSpecializations.All(spec => spec.UserId != userId) &&
                specialization.SpecializationTemplateDepartments
                    .Any(sd => sd.DepartmentId == user.DepartmentId) &&
                specialization.IsActive)
            .OrderByDescending(template => template.Id)
            .Select(specialization => new SpecializationTemplateDto
            {
                Id = specialization.Id,
                Name = specialization.Name
            })
            .ToListAsync(cancellationToken);
    }

    private Task<List<SpecializationTemplateDto>> GetSpecializationTemplateUserHas(int userId,
        CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.UserSpecializations
            .Where(us => us.UserId == userId && us.SpecializationTemplate!.IsActive)
            .OrderByDescending(x => x.SpecializationTemplateId)
            .Select(us => new SpecializationTemplateDto
            {
                Id = us.SpecializationTemplateId,
                Name = us.SpecializationTemplate!.Name,
            }
            )
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(int UserId, bool? AreAssgined) : IQuery<List<SpecializationTemplateDto>>;
}
