using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpUserSnapshot
{
    public int SnapshotId { get; set; }
    public DateOnly SnapshotDate { get; set; }

    public string? SoftSkillSeniority { get; set; }
    public int? SoftSkillSeniorityOrderWeight { get; set; }

    internal static readonly Expression<Func<UserSoftSkill, OdpUserSnapshot>> Map = uss => new OdpUserSnapshot
    {
        SnapshotDate = uss.UserSoftSkillsSnapshot!.SnapshotDate,
        SnapshotId = uss.UserSoftSkillsSnapshot!.Id,
        SoftSkillSeniority = uss.SeniorityLevel!.ExternalName,
        SoftSkillSeniorityOrderWeight = uss.SeniorityLevel!.OrderWeight
    };
}
