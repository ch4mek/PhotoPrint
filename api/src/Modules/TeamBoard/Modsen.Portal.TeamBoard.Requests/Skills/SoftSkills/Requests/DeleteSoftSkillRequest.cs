using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests;

internal sealed class DeleteSoftSkillRequest : ICommandHandler<DeleteSoftSkillRequest.Command, SoftSkillDto>
{
    private readonly IGetSoftSkillDbQuery _getSoftSkillDbQuery;
    private readonly ITeamBoardDbContext _dbContext;

    public DeleteSoftSkillRequest(IGetSoftSkillDbQuery getSoftSkillDbQuery, ITeamBoardDbContext dbContext)
    {
        _getSoftSkillDbQuery = getSoftSkillDbQuery;
        _dbContext = dbContext;
    }

    public async Task<SoftSkillDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var softSkill = await _getSoftSkillDbQuery.ExecuteAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException();

        softSkill.IsActive = false;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SoftSkillDto
        {
            Id = softSkill.Id,
            Name = softSkill.Name
        };
    }

    internal sealed record Command(int Id) : ICommand<SoftSkillDto>, ITransactionalRequest;
}
