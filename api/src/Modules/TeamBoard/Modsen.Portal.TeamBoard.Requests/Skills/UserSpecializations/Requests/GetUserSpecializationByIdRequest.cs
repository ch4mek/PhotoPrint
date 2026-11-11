using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Loaders;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests;

internal sealed class GetUserSpecializationByIdRequest
    : IQueryHandler<GetUserSpecializationByIdRequest.Query, UserSpecializationCoreDto>
{
    private readonly IUserSpecializationLoader _userSpecializationLoader;

    public GetUserSpecializationByIdRequest(IUserSpecializationLoader userSpecializationLoader)
    {
        _userSpecializationLoader = userSpecializationLoader;
    }

    public async Task<UserSpecializationCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var result = await _userSpecializationLoader.Load(request.Id, cancellationToken);

        if (result is null)
        {
            throw new EntityNotFoundException($"Specializations for user with id {request.Id} not found");
        }

        return result;
    }

    internal sealed record Query(int Id) : IQuery<UserSpecializationCoreDto>;
}
