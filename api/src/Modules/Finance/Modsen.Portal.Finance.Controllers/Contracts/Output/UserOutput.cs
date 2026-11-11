using Modsen.Portal.Finance.Requests.Core.CoreDto;

namespace Modsen.Portal.Finance.Controllers.Contracts.Output;

public sealed class UserOutput
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? PatronymicRu { get; set; }
    public string? DepartmentId { get; set; }
    public bool IsActive { get; set; }

    internal static UserOutput Of(UserCoreDto source)
    {
        return new UserOutput
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            FirstNameRu = source.FirstNameRu,
            LastNameRu = source.LastNameRu,
            PatronymicRu = source.PatronymicRu,
            DepartmentId = source.DepartmentId,
            IsActive = source.IsActive
        };
    }
}
