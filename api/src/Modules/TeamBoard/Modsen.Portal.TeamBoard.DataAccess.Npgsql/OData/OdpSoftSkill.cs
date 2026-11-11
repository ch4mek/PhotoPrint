using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpSoftSkill
{
    public int Id { get; set; }
    public required string Name { get; set; }

    internal static readonly Expression<Func<SoftSkill, OdpSoftSkill>> Map =
        x => new OdpSoftSkill
        {
            Id = x.Id,
            Name = x.Name
        };
}
