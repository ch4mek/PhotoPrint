using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Dto;
using Modsen.Portal.Equipment.Requests.EquipmentHistory.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.EquipmentHistory.Requests;
using EquipmentUser = Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities.EquipmentUser;

namespace Modsen.Portal.Equipment.UnitTests.UseCases.EquipmentTests.Commands;

public class SetReturnDateToAssignCommandTests
{
    private readonly Mock<IIsEquipmentExistByIdDbQuery> _isEquipmentExistByIdDbQueryMock = new();
    private readonly Mock<IEquipmentDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly SetReturnDateToAssignRequest _handler;

    public SetReturnDateToAssignCommandTests()
    {
        _handler = new SetReturnDateToAssignRequest(
            _isEquipmentExistByIdDbQueryMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task SetReturnDateToAssignWhenEquipmentDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var populateReturnDateCommand = new SetReturnDateToAssignRequest.Command(
            8,
            new DateTime(2023, 7, 12, 0, 0, 0, DateTimeKind.Utc));

        _isEquipmentExistByIdDbQueryMock
            .Setup(x => x.ExecuteAsync(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var act = () => _handler.Handle(
            populateReturnDateCommand,
            _cancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.EquipmentNotFound);
    }

    [Fact]
    public async Task SetReturnDateToAssignWhenEquipmentIsNotAssignedThrowInvalidDataAppException()
    {
        // Arrange
        var populateReturnDateCommand = new SetReturnDateToAssignRequest.Command(
            8,
            new DateTime(2023, 7, 12, 0, 0, 0, DateTimeKind.Utc));

        _isEquipmentExistByIdDbQueryMock
            .Setup(x => x.ExecuteAsync(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.EquipmentAssigns)
            .Returns(new List<EquipmentAssignEntity>().BuildMockDbSet().Object);

        // Act
        var act = () => _handler.Handle(
            populateReturnDateCommand,
            _cancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.EquipmentWasNeverAssigned);
    }

    [Fact]
    public async Task SetReturnDateToAssignWhenEquipmentHasSeveralAssignsWithoutReturnDateThrowInvalidDataAppException()
    {
        // Arrange
        var populateReturnDateCommand = new SetReturnDateToAssignRequest.Command(
            8,
            new DateTime(2023, 7, 12, 0, 0, 0, DateTimeKind.Utc));

        var assignsWithoutReturnDate = Enumerable.Repeat(new EquipmentAssignEntity()
        {
            EquipmentId = populateReturnDateCommand.EquipmentId,
            ReturnDate = null
        }, 2).ToList();

        _isEquipmentExistByIdDbQueryMock
            .Setup(x => x.ExecuteAsync(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.EquipmentAssigns)
            .Returns(new List<EquipmentAssignEntity>(assignsWithoutReturnDate).BuildMockDbSet().Object);

        // Act
        var act = () => _handler.Handle(
            populateReturnDateCommand,
            _cancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.SeveralNullDatesForEquipment);
    }

    [Fact]
    public async Task SetReturnDateToAssignWhenReturnDateAlreadyExistsShouldThrowEntityConflictException()
    {
        // Arrange
        var populateReturnDateCommand = new SetReturnDateToAssignRequest.Command(
            12,
            new DateTime(2023, 7, 12, 0, 0, 0, DateTimeKind.Utc));

        var assigns = new List<EquipmentAssignEntity>
        {
            new()
            {
                EquipmentId = populateReturnDateCommand.EquipmentId,
                ReturnDate = new DateTime(2022, 6, 12, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        _isEquipmentExistByIdDbQueryMock
            .Setup(x => x.ExecuteAsync(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.EquipmentAssigns)
            .Returns(new List<EquipmentAssignEntity>(assigns).BuildMockDbSet().Object);

        // Act
        var act = () => _handler.Handle(
            populateReturnDateCommand,
            _cancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<EntityConflictException>()
            .WithMessage(PortalMessages.ExistingReturnDateForLastAssignment);
    }

    [Fact]
    public async Task SetReturnDateToAssignWhenReturnDateIsLessThanIssueDateThrowInvalidDataAppException()
    {
        // Arrange
        var returnDate = new DateTime(2023, 7, 12, 0, 0, 0, DateTimeKind.Utc);

        var populateReturnDateCommand = new SetReturnDateToAssignRequest.Command(
            12,
            returnDate);

        var assigns = new List<EquipmentAssignEntity>
        {
            new()
            {
                EquipmentId = populateReturnDateCommand.EquipmentId,
                IssueDate = returnDate.AddDays(1),
                ReturnDate = null
            }
        };

        _isEquipmentExistByIdDbQueryMock
            .Setup(x => x.ExecuteAsync(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.EquipmentAssigns)
            .Returns(new List<EquipmentAssignEntity>(assigns).BuildMockDbSet().Object);

        // Act
        var act = () => _handler.Handle(
            populateReturnDateCommand,
            _cancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.ReturnDateLowerThanIssueDate);
    }

    [Fact]
    public async Task SetReturnDateToAssignWhenDataIsValidReturnUpdatedEquipmentModel()
    {
        // Arrange
        var returnDate = new DateTime(2023, 7, 12, 0, 0, 0, DateTimeKind.Utc);

        var populateReturnDateCommand = new SetReturnDateToAssignRequest.Command(
            12,
            returnDate);

        var assign = new EquipmentAssignEntity
        {
            EquipmentId = populateReturnDateCommand.EquipmentId,
            IssueDate = returnDate.AddDays(-1),
            ReturnDate = null,
            IsDeleted = false,
            User = new EquipmentUser
            {
                FirstName = "Test",
                LastName = "User",
                DepartmentId = "QA",
                Email = "test",
            }
        };

        var expectedHolder = new EquipmentHolderDto
        {
            Id = 0,
            FirstName = "Test",
            LastName = "User",
            DepartmentId = "QA",
            IssueDate = assign.IssueDate,
            ReturnDate = returnDate,
        };

        _isEquipmentExistByIdDbQueryMock
            .Setup(x => x.ExecuteAsync(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.EquipmentAssigns)
            .Returns(new List<EquipmentAssignEntity> { assign }.BuildMockDbSet().Object);

        // Act
        var act = await _handler.Handle(
            populateReturnDateCommand,
            _cancellationToken);

        // Assert
        act.Should().BeEquivalentTo(expectedHolder);
    }

}
