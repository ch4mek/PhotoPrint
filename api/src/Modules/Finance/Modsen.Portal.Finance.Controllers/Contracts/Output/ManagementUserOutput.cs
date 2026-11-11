using System.Collections.ObjectModel;
using Modsen.Portal.Finance.Requests.Managements.Requests.RequestDtos;

namespace Modsen.Portal.Finance.Controllers.Contracts.Output;

public sealed class ManagementUserOutput
{
    public int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? FirstNameRu { get; init; }
    public string? LastNameRu { get; init; }
    public string? DepartmentId { get; init; }
    public bool IsActive { get; init; }

    public Collection<ManagementProjectOutput> Projects { get; init; } = [];

    public decimal? BillableTrackedHoursAmount { get; init; }
    public decimal? HourlyRateAmount { get; init; }
    public decimal? IncomeAmount { get; init; }
    public decimal? IncomeBynAmount { get; init; }
    public decimal? Salary { get; init; }
    public int Kpi { get; init; }
    public string? KpiColor { get; init; }

    public bool CanUserInfoBeRetrievedByCurrentUser { get; init; }

    internal static ManagementUserOutput Of(ManagementUserDto source)
    {
        return new ManagementUserOutput
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            FirstNameRu = source.FirstNameRu,
            LastNameRu = source.LastNameRu,
            DepartmentId = source.DepartmentId,
            IsActive = source.IsActive,
            BillableTrackedHoursAmount = source.BillableTrackedHoursAmount != decimal.Zero ? source.BillableTrackedHoursAmount : null,
            HourlyRateAmount = source.HourlyRateAmount != decimal.Zero ? source.HourlyRateAmount : null,
            IncomeAmount = source.IncomeAmount != decimal.Zero ? source.IncomeAmount : null,
            IncomeBynAmount = source.IncomeBynAmount != decimal.Zero ? source.IncomeBynAmount : null,
            Salary = source.Salary,
            Kpi = source.Kpi,
            KpiColor = source.KpiColor,
            CanUserInfoBeRetrievedByCurrentUser = source.CanUserInfoBeRetrievedByCurrentUser,
            Projects = new Collection<ManagementProjectOutput>([.. source.Projects.Select(ManagementProjectOutput.Of)])
        };
    }
}
