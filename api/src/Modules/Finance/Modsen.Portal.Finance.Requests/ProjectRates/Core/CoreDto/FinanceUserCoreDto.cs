using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

public class FinanceUserCoreDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? PatronymicRu { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
    public bool IsActive { get; set; }

    internal static FinanceUserCoreDto? Of(FinanceUserEntity? source)
    {
        if(source is null)
        {
            return null;
        }

        return new FinanceUserCoreDto
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            FirstNameRu = source.FirstNameRu,
            LastNameRu = source.LastNameRu,
            PatronymicRu = source.PatronymicRu,
            Email = source.Email,
            DepartmentId = source.DepartmentId,
            IsActive = source.IsActive,
        };
    }        
}


