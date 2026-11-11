using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests;

internal sealed class GetSoftSkillByIdRequest : IQueryHandler<GetSoftSkillByIdRequest.Query, SoftSkillDto>
{
    private readonly ITeamBoardDbContext _dbContext;

    public GetSoftSkillByIdRequest(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SoftSkillDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var result = await _dbContext.SoftSkills
            .Where(softSkill => softSkill.Id == request.Id && softSkill.IsActive)
            .Select(skill => new SoftSkillDto
            {
                Id = skill.Id,
                Name = skill.Name
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException($"Soft skill by id {request.Id} is not found");

        return result;
    }

    internal sealed record Query(int Id) : IQuery<SoftSkillDto>;
}
