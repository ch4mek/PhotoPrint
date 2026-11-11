using MockQueryable.Moq;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Payload.Core;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Payload.Core;

public class TotalEmployeesPayloadCalculatorTests
{
    private readonly ITotalEmployeesPayloadCalculator _totalEmployeesPayloadCalculator;
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<IKpiColorCalculator> _kpiColorCalculatorMock = new();
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();

    public TotalEmployeesPayloadCalculatorTests()
    {
        _totalEmployeesPayloadCalculator = new TotalEmployeesPayloadCalculator(
            _clockServiceMock.Object,
            _kpiColorCalculatorMock.Object,
            _dbContextMock.Object);

        _dbContextMock.Setup(x => x.KpiColors)
            .Returns(new List<KpiColor>()
            {
                new()
                {
                    Id = 1,
                    Name = "Green",
                    RightIntervalBorder = 100,
                    ColorOrderWeight = 1
                }
            }.BuildMockDbSet().Object);

        _kpiColorCalculatorMock
            .Setup(mock => mock.GetKpiColor(It.IsAny<decimal>(), It.IsAny<List<KpiColorDto>>()))
            .Returns("Green");

        _clockServiceMock
            .Setup(mock => mock.UtcNow)
            .Returns(new DateTime(2024, 3, 2));
    }

    [Fact]
    public async Task CalculateWhenDataCorrectShouldReturnTotalValues()
    {
        //Arrange
        var payload = new PayloadCoreDto()
        {
            Users =
            [
                new() {
                    FirstName = "test",
                    LastName = "test",
                    HourDetails =
                    [
                        new() {
                            MonthOfYear = 1,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            TotalUserTime = 100,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 2,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            TotalUserTime = 100,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 3,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            TotalUserTime = 100,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 4,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            TotalUserTime = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 5,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 6,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 7,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 8,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 9,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 10,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 11,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 12,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                    ],
                },
                new() {
                    FirstName = "test",
                    LastName = "test",
                    HourDetails =
                    [
                        new() {
                            MonthOfYear = 1,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            TotalUserTime = 100,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 2,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            TotalUserTime = 100,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 3,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            TotalUserTime = 100,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 4,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            TotalUserTime = 30,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 5,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 6,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 7,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 8,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 9,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 10,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 11,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                        new() {
                            MonthOfYear = 12,
                            RequiredHours = 100,
                            TrackedBillableInHours = 10,
                            PlannedWorkloadInHours = 30,
                            ActualFte = 1,
                            PlannedFte = 1,
                            HasActivity = true
                        },
                    ],
                },
            ],
        };

        //Act
        var result = await _totalEmployeesPayloadCalculator.Calculate(payload, CancellationToken.None);

        //Assert

        result.Length.Should().Be(12);

        result.First(month => month.MonthOfYear == 1).TotalPlannedWorkloadInHours.Should().Be(200);
        result.First(month => month.MonthOfYear == 1).TotalFactBillableInHours.Should().Be(20);
        result.First(month => month.MonthOfYear == 1).TotalPlannedFte.Should().Be(2);
        result.First(month => month.MonthOfYear == 1).TotalActualFte.Should().Be(2);
        result.First(month => month.MonthOfYear == 1).TotalKpi.Should().Be(10);

        result.First(month => month.MonthOfYear == 2).TotalPlannedWorkloadInHours.Should().Be(200);
        result.First(month => month.MonthOfYear == 2).TotalFactBillableInHours.Should().Be(20);
        result.First(month => month.MonthOfYear == 2).TotalPlannedFte.Should().Be(2);
        result.First(month => month.MonthOfYear == 2).TotalActualFte.Should().Be(2);
        result.First(month => month.MonthOfYear == 2).TotalKpi.Should().Be(10);

        result.First(month => month.MonthOfYear == 3).TotalPlannedWorkloadInHours.Should().Be(200);
        result.First(month => month.MonthOfYear == 3).TotalFactBillableInHours.Should().Be(20);
        result.First(month => month.MonthOfYear == 3).TotalPlannedFte.Should().Be(2);
        result.First(month => month.MonthOfYear == 3).TotalActualFte.Should().Be(2);
        result.First(month => month.MonthOfYear == 3).TotalKpi.Should().Be(10);

        result.First(month => month.MonthOfYear == 4).TotalPlannedWorkloadInHours.Should().Be(60);
        result.First(month => month.MonthOfYear == 4).TotalFactBillableInHours.Should().Be(20);
        result.First(month => month.MonthOfYear == 4).TotalPlannedFte.Should().Be(2);
        result.First(month => month.MonthOfYear == 4).TotalActualFte.Should().Be(2);
        result.First(month => month.MonthOfYear == 4).TotalKpi.Should().Be(33.33m);
    }
}
