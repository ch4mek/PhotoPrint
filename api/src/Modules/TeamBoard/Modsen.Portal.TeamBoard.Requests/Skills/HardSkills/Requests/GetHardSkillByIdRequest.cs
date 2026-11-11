using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests;

internal sealed class GetHardSkillByIdRequest : IQueryHandler<GetHardSkillByIdRequest.Query, HardSkillDto>
{
    private readonly ITeamBoardDbContext _dbContext;

    public GetHardSkillByIdRequest(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HardSkillDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var hardSkillDto = await _dbContext.HardSkills
                .Include(skill => skill.Group)
                .Where(skill => skill.Id == request.Id && skill.IsActive)
                .Select(skill => new HardSkillDto
                {
                    Id = skill.Id,
                    Name = skill.Name,
                    HardSkillGroupId = skill.HardSkillGroupId,
                    HardSkillGroupName = skill.Group!.Name
                })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new EntityNotFoundException();

        return hardSkillDto;
    }

    internal sealed record Query(int Id) : IQuery<HardSkillDto>;
}
