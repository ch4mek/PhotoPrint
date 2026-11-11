using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.Managements.Core.CoreDto;

internal class ManagementsOrderingCoreDto
{
    public ManagementsOrderingField Field { get; init; }
    public bool IsDescendingOrdering { get; init; }

    public bool ShouldOrderByStatistics =>
        Field == ManagementsOrderingField.TrackedHours ||
        Field == ManagementsOrderingField.IncomeByn ||
        Field == ManagementsOrderingField.Kpi;

    public FinanceOrderingCoreDto Map()
    {
        return new FinanceOrderingCoreDto
        {
            IsDescending = IsDescendingOrdering,
            Field = Field switch
            {
                ManagementsOrderingField.UserFullName => FinanceOrderingField.UserFullName,
                ManagementsOrderingField.DepartmentId => FinanceOrderingField.DepartmentId,
                _ => FinanceOrderingField.UserId
            }
        };
    }
}

public enum ManagementsOrderingField
{
    UserId = 0,
    UserFullName = 1,
    DepartmentId = 2,
    TrackedHours = 3,
    IncomeByn = 5,
    Kpi = 6,
}
