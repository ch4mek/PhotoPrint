using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests;

internal sealed class GetUserSpecializationsByUserIdRequest
    : IQueryHandler<GetUserSpecializationsByUserIdRequest.Query, UserSpecializationTemplateWithHardSkillsDto[]>
{
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetUserSpecializationsByUserIdRequest(
        ITeamBoardDbContext teamBoardDbContext,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
    }

    public async Task<UserSpecializationTemplateWithHardSkillsDto[]> Handle(Query request,
        CancellationToken cancellationToken)
    {
        var canUserBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule
            .IsCorrespondsToBusiness(request.UserId, cancellationToken);
        if (!canUserBeRetrieved)
        {
            throw new EntityNotFoundException();
        }

        var userSpecializations = await _teamBoardDbContext.UserSpecializations
            .AsNoTracking()
            .Include(x => x.SeniorityLevel)
            .Include(x => x.SpecializationTemplate)
            .Include(x => x.User)
            .Where(x => x.UserId == request.UserId && x.User!.IsActive)
            .Where(x => request.SpecializationTemplateName == null ||
                EF.Functions.Like(x.SpecializationTemplate!.Name, $"%{(request.SpecializationTemplateName ?? string.Empty).ToLower(CultureInfo.InvariantCulture)}%"))
            .OrderByDescending(x => x.IsPrimary)
                .ThenByDescending(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

        var result = userSpecializations
           .Select(us =>
           {
               var hardSkills = _teamBoardDbContext.UserSkillsSnapshotSpecializations
                   .Where(uss =>
                       uss.SpecializationTemplateId == us.SpecializationTemplate!.Id &&
                       uss.UserSkillsSnapshot!.UserId == request.UserId)
                   .SelectMany(uss => uss.HardSkills)
                   .Select(snapshotSpecializationHardSkill => new ShortHardSkillDto
                   {
                       Id = snapshotSpecializationHardSkill.HardSkillId,
                       Name = snapshotSpecializationHardSkill.HardSkill!.Name,
                   })
                   .Distinct()
                   .ToArray();

               return new UserSpecializationTemplateWithHardSkillsDto
               {
                   Id = us.Id,
                   IsPrimary = us.IsPrimary,
                   SpecializationTemplateId = us.SpecializationTemplateId,
                   SpecializationTemplateName = us.SpecializationTemplate!.Name,
                   SeniorityLevelId = us.SeniorityLevelId,
                   SeniorityLevelName = us.SeniorityLevel!.InternalName,
                   HardSkills = hardSkills
               };
           })
           .ToArray();

        return result;
    }

    internal sealed record Query(int UserId, string? SpecializationTemplateName)
        : IQuery<UserSpecializationTemplateWithHardSkillsDto[]>;
}
