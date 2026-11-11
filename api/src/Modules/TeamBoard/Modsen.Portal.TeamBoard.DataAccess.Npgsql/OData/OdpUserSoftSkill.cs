
using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpUserSoftSkill
{
    public int Id { get; set; }
    public required string SoftSkillName { get; set; }
    public IReadOnlyCollection<OdpUserSnapshot> Snapshots { get; set; } = [];

    internal static readonly Expression<Func<IGrouping<SoftSkill, UserSoftSkill>, OdpUserSoftSkill>> Map = uss =>
        new OdpUserSoftSkill
        {
            Id = uss.Key!.Id,
            SoftSkillName = uss.Key!.Name,
            Snapshots = uss.AsQueryable().Select(OdpUserSnapshot.Map).ToList()
        };
}
