using Microsoft.AspNetCore.Identity;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class Role : IdentityRole<int>
{
    public int OrderWeight { get; set; }
    public bool IsCompanyManager { get; set; }
    public required string PublicName { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RoleClaim> Claims { get; set; } = [];
}
