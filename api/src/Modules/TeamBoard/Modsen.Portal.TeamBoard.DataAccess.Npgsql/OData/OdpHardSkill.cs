using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpHardSkill
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string HardSkillGroupId { get; set; }

    internal static readonly Expression<Func<HardSkill, OdpHardSkill>> Map =
        x => new OdpHardSkill
        {
            Id = x.Id,
            Name = x.Name,
            HardSkillGroupId = x.HardSkillGroupId,
        };
}
