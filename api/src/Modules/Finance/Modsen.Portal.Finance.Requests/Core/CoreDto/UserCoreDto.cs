using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Finance.Requests.Core.CoreDto;

internal class UserCoreDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? PatronymicRu { get; set; }
    public string? DepartmentId { get; set; }
    public bool IsActive { get; set; }

    public static UserCoreDto Of(FinanceUserEntity source, string? historicalDepartmentId = null)
    {
        return new UserCoreDto
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            FirstNameRu = source.FirstNameRu,
            LastNameRu = source.LastNameRu,
            PatronymicRu = source.PatronymicRu,
            DepartmentId = historicalDepartmentId ?? source.DepartmentId,
            IsActive = source.IsActive
        };
    }
}
