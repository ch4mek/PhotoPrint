using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpNonPrimarySpecialization
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int UserId { get; set; }

    public string? SeniorityLevelId { get; set; }
    public string? SeniorityLevel { get; set; }
    public int OrderWeight { get; set; }

    public bool IsVisibleSpecialization { get; set; }
    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    internal static readonly Expression<Func<UserSpecialization, OdpNonPrimarySpecialization>> MapNonPrimarySpecialization =
        x => new OdpNonPrimarySpecialization
        {
            Id = x.Id,
            Name = x.SpecializationTemplate!.Name,
            UserId = x.UserId,
            SeniorityLevelId = x.SeniorityLevelId,
            SeniorityLevel = x.SeniorityLevel!.InternalName,
            OrderWeight = x.SeniorityLevel.OrderWeight,
            IsActive = x.SpecializationTemplate!.IsActive,
            IsVisibleSpecialization = x.IsVisibleSpecialization,
            CreatedAtUtc = x.CreatedAtUtc
        };
}
