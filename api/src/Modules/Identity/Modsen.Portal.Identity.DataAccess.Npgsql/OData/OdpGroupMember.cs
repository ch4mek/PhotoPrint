using System.Linq.Expressions;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.OData;

public class OdpGroupMember
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; }

    internal static readonly Expression<Func<User, OdpGroupMember>> Map =
        x => new OdpGroupMember
        {
            Id = x.Id,
            FullName = x.LastName + ' ' + x.FirstName,
            Email = x.Email!,
            Location = x.Location.ToString(),
            IsActive = x.IsActive,
        };
}
