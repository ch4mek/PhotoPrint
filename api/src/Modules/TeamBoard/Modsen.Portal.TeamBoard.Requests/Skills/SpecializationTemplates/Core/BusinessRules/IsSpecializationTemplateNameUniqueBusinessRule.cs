using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;

internal interface IIsSpecializationTemplateNameUniqueBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(string name, int? id, CancellationToken cancellationToken);
}

internal sealed class IsSpecializationTemplateNameUniqueBusinessRule : IIsSpecializationTemplateNameUniqueBusinessRule
{
    private readonly ITeamBoardDbContext _dbContext;

    public IsSpecializationTemplateNameUniqueBusinessRule(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(string name, int? id, CancellationToken cancellationToken)
    {
        return !await _dbContext.SpecializationTemplates.AnyAsync(
            specialization =>
                EF.Functions.ILike(specialization.Name, name) &&
                specialization.IsActive &&
                specialization.Id != id,
            cancellationToken);
    }
}
