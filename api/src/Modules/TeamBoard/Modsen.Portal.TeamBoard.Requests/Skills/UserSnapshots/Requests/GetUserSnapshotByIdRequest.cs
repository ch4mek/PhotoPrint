using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Requests;

internal sealed class GetUserSnapshotByIdRequest
    : IQueryHandler<GetUserSnapshotByIdRequest.Query, UserSnapshotCoreDto>
{
    private readonly IUserSkillsSnapshotLoader _userSkillsSnapshotLoader;

    public GetUserSnapshotByIdRequest(IUserSkillsSnapshotLoader userSkillsSnapshotLoader)
    {
        _userSkillsSnapshotLoader = userSkillsSnapshotLoader;
    }

    public async Task<UserSnapshotCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var userSnapshot = await _userSkillsSnapshotLoader.Load(request.Id, cancellationToken);

        if (userSnapshot is null)
        {
            throw new EntityNotFoundException($"Entry with id {request.Id} not found");
        }

        return userSnapshot;
    }

    internal sealed record Query(int Id) : IQuery<UserSnapshotCoreDto>;

}
