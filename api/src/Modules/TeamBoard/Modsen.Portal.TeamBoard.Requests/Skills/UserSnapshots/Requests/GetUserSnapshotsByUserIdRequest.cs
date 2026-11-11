using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Requests;

internal sealed class GetUserSnapshotsByUserIdRequest : IQueryHandler<GetUserSnapshotsByUserIdRequest.Query,
    PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>>
{
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IUserSkillsSnapshotsLoader _userSkillsSnapshotsLoader;

    public GetUserSnapshotsByUserIdRequest(
        ITeamBoardDbContext teamBoardDbContext,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        IUserSkillsSnapshotsLoader userSkillsSnapshotsLoader)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _userSkillsSnapshotsLoader = userSkillsSnapshotsLoader;
    }

    public async Task<PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>> Handle(Query request,
        CancellationToken cancellationToken)
    {
        var canBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(
            request.UserId, cancellationToken);

        if (!canBeRetrieved)
        {
            throw new EntityNotFoundException();
        }

        var userSnapshots = await _userSkillsSnapshotsLoader.Load(request.UserId, cancellationToken);

        var queryOptions = request.QueryOptions;

        var userSpecializationsQuery = _teamBoardDbContext.UserSpecializations
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId && x.User!.IsActive)
            .Select(UserSpecializationTemplateWithSnapshotDto.Map)
            .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.CreatedAtUtc);

        var userSpecializations = await queryOptions
            .ApplyTo(userSpecializationsQuery)
            .Cast<UserSpecializationTemplateWithSnapshotDto>()
            .ToListAsync(cancellationToken);

        foreach (var us in userSpecializations)
        {
            var filteredSnapshots = userSnapshots
                .Where(snapshot => snapshot.UserSpecializations
                    .Any(sp => sp.SpecializationTemplateId == us.SpecializationTemplateId));

            us.SpecializationSnapshots = filteredSnapshots
                .Select(snapshot => new UserSnapshotCoreDto
                {
                    Id = snapshot.Id,
                    UserId = snapshot.UserId,
                    SnapshotDate = snapshot.SnapshotDate,

                    UserSpecializations = snapshot.UserSpecializations
                        .Where(sp => sp.SpecializationTemplateId == us.SpecializationTemplateId)
                        .ToArray()
                })
                .ToList();
        }

        var total = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(userSpecializationsQuery)
            .Cast<UserSpecializationTemplateWithSnapshotDto>()
            .CountAsync(cancellationToken);

        return userSpecializations.ToPageable(queryOptions, total);
    }

    internal sealed record Query(int UserId, ODataQueryOptions<UserSpecializationTemplateWithSnapshotDto> QueryOptions)
        : IQuery<PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
