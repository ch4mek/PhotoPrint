using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Finance.Requests.Core.CoreDto;

public class ExpenseCategoryCoreDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int OrderWeight { get; set; }
    public required bool IsDeleted { get; set; }

    internal static ExpenseCategoryCoreDto Of(ExpenseCategoryEntity source)
    {
        return new ExpenseCategoryCoreDto
        {
            Id = source.Id,
            Name = source.Name,
            OrderWeight = source.OrderWeight,
            IsDeleted = source.IsDeleted,
        };
    }
}

