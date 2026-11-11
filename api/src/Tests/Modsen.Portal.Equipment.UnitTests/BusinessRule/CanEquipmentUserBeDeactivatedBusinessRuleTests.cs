using MockQueryable.Moq;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Core.BusinessRules;

namespace Modsen.Portal.Equipment.UnitTests.BusinessRule;

public class CanEquipmentUserBeDeactivatedBusinessRuleTests
{
    private readonly Mock<IEquipmentDbContext> _dbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();

    private readonly CanEquipmentUserBeDeactivatedBusinessRule _canEquipmentUserBeDeactivatedBusinessRule;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CanEquipmentUserBeDeactivatedBusinessRuleTests()
    {
        _canEquipmentUserBeDeactivatedBusinessRule = new CanEquipmentUserBeDeactivatedBusinessRule(
            _dbContextMock.Object,
            _clockServiceMock.Object);
    }

    [Fact]
    public async Task IsCorrespondingToBusinessWhenUserHasNoActiveEquipmentAssignmentsAndUserIsNotApproverOfEquipmentShouldReturnTrue()
    {
        //Arrange
        const int userId = 1;

        _clockServiceMock.Setup(x => x.UtcNow)
            .Returns(DateTime.UtcNow);

        _dbContextMock.Setup(x => x.EquipmentAssigns)
            .Returns(new List<EquipmentAssignEntity>().BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.Equipments)
            .Returns(new List<EquipmentEntity>().BuildMockDbSet().Object);

        //Act
        var actualResult = await _canEquipmentUserBeDeactivatedBusinessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        actualResult.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondingToBusinessWhenUserHasActiveEquipmentAssignmentsShouldReturnFalse()
    {
        //Arrange
        const int userId = 1;

        _clockServiceMock.Setup(x => x.UtcNow)
            .Returns(DateTime.UtcNow);

        _dbContextMock.Setup(x => x.EquipmentAssigns)
            .Returns(new List<EquipmentAssignEntity>
            {
                new()
                {
                    IsDeleted = false,
                    AssignedToUserId = userId,
                    ReturnDate = null
                }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.Equipments)
            .Returns(new List<EquipmentEntity>().BuildMockDbSet().Object);

        //Act
        var actualResult = await _canEquipmentUserBeDeactivatedBusinessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        actualResult.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondingToBusinessWhenUserIsApproverOfEquipmentShouldReturnFalse()
    {
        //Arrange
        const int userId = 1;

        _clockServiceMock.Setup(x => x.UtcNow)
            .Returns(DateTime.UtcNow);

        _dbContextMock.Setup(x => x.EquipmentAssigns)
            .Returns(new List<EquipmentAssignEntity>().BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.Equipments)
            .Returns(new List<EquipmentEntity>
            {
                new()
                {
                    ApproverId = userId,
                    BrokenStatus = EquipmentBrokenStatus.None,
                    Name = "test",
                    SerialNumber = "123",
                    PurchasePlace = "Amazon",
                    Characteristics = "Intel Core i7, 16GB RAM, 512GB SSD"
                }
            }.BuildMockDbSet().Object);

        //Act
        var actualResult = await _canEquipmentUserBeDeactivatedBusinessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        actualResult.Should().BeFalse();
    }
}
