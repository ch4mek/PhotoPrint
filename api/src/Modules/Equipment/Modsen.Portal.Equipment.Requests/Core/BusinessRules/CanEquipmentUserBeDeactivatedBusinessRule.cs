using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Equipment.DataAccess.Npgsql;

namespace Modsen.Portal.Equipment.Requests.Core.BusinessRules;

internal interface ICanEquipmentUserBeDeactivatedBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken);
}

internal class CanEquipmentUserBeDeactivatedBusinessRule : ICanEquipmentUserBeDeactivatedBusinessRule
{
    private readonly IEquipmentDbContext _dbContext;
    private readonly IClockService _clockService;

    private CancellationToken _cancellationToken;
    private int _userId;

    public CanEquipmentUserBeDeactivatedBusinessRule(IEquipmentDbContext dbContext, IClockService clockService)
    {
        _dbContext = dbContext;
        _clockService = clockService;

    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _userId = userId;

        var hasActiveAssignments = await HasActiveAssignmentsAsync();

        var isUserApprover = await IsUserApproverAsync();

        return !hasActiveAssignments && !isUserApprover;
    }

    private async Task<bool> HasActiveAssignmentsAsync()
    {
        var currentDate = _clockService.UtcNow.Date;

        var hasActiveAssignments = await _dbContext.EquipmentAssigns
            .AsNoTracking()
            .AnyAsync(assignEntity =>
                    !assignEntity.IsDeleted &&
                    assignEntity.AssignedToUserId == _userId &&
                    (assignEntity.ReturnDate == null || assignEntity.ReturnDate.Value.Date > currentDate),
                _cancellationToken);

        return hasActiveAssignments;
    }

    private async Task<bool> IsUserApproverAsync()
    {
        var hasActiveAssignments = await _dbContext.Equipments
            .AnyAsync(equipmentEntity => equipmentEntity.ApproverId == _userId,
                _cancellationToken);

        return hasActiveAssignments;
    }
}
