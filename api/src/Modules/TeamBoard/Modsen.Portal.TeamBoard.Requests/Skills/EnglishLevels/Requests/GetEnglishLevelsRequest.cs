using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache;
using Modsen.Portal.TeamBoard.Requests.Skills.EnglishLevels.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.EnglishLevels.Requests;

internal sealed class GetEnglishLevelsRequest : IQueryHandler<GetEnglishLevelsRequest.Query, List<EnglishLevelDto>>
{
    private readonly IEnglishLevelCache _cache;

    public GetEnglishLevelsRequest(IEnglishLevelCache cache)
    {
        _cache = cache;
    }

    public async Task<List<EnglishLevelDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var result = await _cache.GetAllEnglishLevels(cancellationToken);

        return result.ConvertAll(englishLevel => new EnglishLevelDto
        {
            Id = englishLevel.Id,
            Name = englishLevel.Name
        });
    }

    internal sealed record Query : IQuery<List<EnglishLevelDto>>;
}
