using System.Linq.Expressions;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpUserContract
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateOnly ContractFrom { get; set; }
    public DateOnly? ContractTo { get; set; }

    public DateOnly? FireDate { get; set; }
    public FireType? FireType { get; set; }
    public string? FireReason { get; set; }
    public string? FireDescription { get; set; }

    internal static readonly Expression<Func<UserContract, OdpUserContract>> Map =
        x => new OdpUserContract
        {
            Id = x.Id,
            UserId = x.UserId,
            ContractFrom = x.ContractFrom <= new DateOnly(0001, 1, 1) ? new DateOnly(0001, 1, 2) : x.ContractFrom,
            ContractTo = x.ContractTo,
            FireDate = x.FireDate,
            FireType = x.FireType,
            FireReason = x.FireReason,
            FireDescription = x.FireDescription
        };
}
