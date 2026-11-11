using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.Requests.Core.DbQueries;

internal interface IUserCreatedOrUpdatedDbQuery
{
    Task ExecuteAsync(EquipmentUser equipmentUser, CancellationToken cancellationToken);
}

internal class UserCreatedOrUpdatedDbQuery : IUserCreatedOrUpdatedDbQuery
{
    private readonly IEquipmentDbContext _equipmentDbContext;

    public UserCreatedOrUpdatedDbQuery(IEquipmentDbContext equipmentDbContext)
    {
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task ExecuteAsync(EquipmentUser equipmentUser, CancellationToken cancellationToken)
    {
        var user = await _equipmentDbContext.EquipmentUsers
            .FirstOrDefaultAsync(x => x.UserId == equipmentUser.UserId, cancellationToken);
        
        if (user == null)
        {
            user = new EquipmentUser
            {
                UserId = equipmentUser.UserId,
                IsActive = true,
                FirstName = equipmentUser.FirstName,
                LastName = equipmentUser.LastName,
                Email = equipmentUser.Email
            };

            _equipmentDbContext.EquipmentUsers.Add(user);
        }

        if (equipmentUser.Position != null)
        {
            user.Position = equipmentUser.Position;
        }

        user.FirstName = equipmentUser.FirstName;
        user.LastName = equipmentUser.LastName;
        user.FirstNameRu = equipmentUser.FirstNameRu;
        user.LastNameRu = equipmentUser.LastNameRu;
        user.Email = equipmentUser.Email;
        user.DepartmentId = equipmentUser.DepartmentId;
        user.ResourceGroupId = equipmentUser.ResourceGroupId;

        await _equipmentDbContext.SaveChangesAsync(cancellationToken);
    }
}
