using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;

public interface ISpecializationNotAssignedToAnyUserBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int specializationTemplateId, CancellationToken cancellationToken);
}

internal class SpecializationNotAssignedToAnyUserBusinessRule : ISpecializationNotAssignedToAnyUserBusinessRule
{
    private readonly ITeamBoardDbContext _dbContext;

    public SpecializationNotAssignedToAnyUserBusinessRule(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> IsCorrespondsToBusiness(int specializationTemplateId, CancellationToken cancellationToken)
    {
        return _dbContext.UserSpecializations.AnyAsync(userSpecialization =>
            userSpecialization.SpecializationTemplateId == specializationTemplateId, cancellationToken);
    }
}
