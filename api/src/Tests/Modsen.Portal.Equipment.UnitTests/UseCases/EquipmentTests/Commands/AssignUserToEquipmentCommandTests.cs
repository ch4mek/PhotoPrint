using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Dto;
using Modsen.Portal.Equipment.Requests.EquipmentHistory.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.EquipmentHistory.Requests;

namespace Modsen.Portal.Equipment.UnitTests.UseCases.EquipmentTests.Commands;

public class AssignUserToEquipmentCommandTests
{
    private readonly Mock<IIsEquipmentExistByIdDbQuery> _isEquipmentExistByIdDbQueryMock = new();
    private readonly Mock<IEquipmentDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken;
    private readonly AssignUserToEquipmentRequest _handler;

    public AssignUserToEquipmentCommandTests()
    {
        _cancellationToken = CancellationToken.None;
        _handler = new AssignUserToEquipmentRequest(_isEquipmentExistByIdDbQueryMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenEquipmentDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var equipmentId = 1;
        const int userId = 5;
        const int approverId = 3;
        var issueDate = DateTime.UtcNow.Date;

        var request = new AssignUserToEquipmentRequest.Command(
            equipmentId,
            userId,
            approverId,
            issueDate);

        _isEquipmentExistByIdDbQueryMock.Setup(x => x.ExecuteAsync(
                equipmentId,
                _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.EquipmentNotFound);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var equipmentId = 2;
        const int userId = 5;
        const int approverId = 3;
        var issueDate = DateTime.UtcNow.Date;

        var request = new AssignUserToEquipmentRequest.Command(
            equipmentId,
            approverId,
            userId,
            issueDate);

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>().BuildMockDbSet().Object);

        _isEquipmentExistByIdDbQueryMock
            .Setup(x => x.ExecuteAsync(equipmentId, _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenApproverIsHeadOfAnotherDepartmentShouldThrowActionNotAllowedException()
    {
        // Arrange
        var equipmentId = 2;
        const int userId = 5;
        const int approverId = 3;
        var issueDate = DateTime.UtcNow.Date;

        var request = new AssignUserToEquipmentRequest.Command(
            equipmentId,
            approverId,
            userId,
            issueDate);

        var user = new EquipmentUser
        {
            UserId = userId,
            FirstName = Guid.CreateVersion7().ToString(),
            LastName = Guid.CreateVersion7().ToString(),
            Email = $"{Guid.CreateVersion7()}modsen-software.com",
            DepartmentId = "Net",
            Position = PortalRoleNames.Employee,
            IsActive = true
        };

        var approver = new EquipmentUser
        {
            UserId = approverId,
            FirstName = Guid.CreateVersion7().ToString(),
            LastName = Guid.CreateVersion7().ToString(),
            Email = $"{Guid.CreateVersion7()}modsen-software.com",
            Position = PortalRoleNames.HeadOfDepartment,
            DepartmentId = "Java",
            IsActive = true
        };

        _isEquipmentExistByIdDbQueryMock
            .Setup(x => x.ExecuteAsync(equipmentId, _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>
            {
                user, approver
            }.BuildMockDbSet().Object);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.ApproverDoesNotHavePermission);
    }

    [Fact]
    public async Task HandleWhenApproverDoesNotHaveManagerRoleShouldThrowActionNotAllowedException()
    {
        // Arrange
        var equipmentId = 2;
        const int userId = 5;
        const int approverId = 3;
        var issueDate = DateTime.UtcNow.Date;

        var request = new AssignUserToEquipmentRequest.Command(
            equipmentId,
            approverId,
            userId,
            issueDate);

        var user = new EquipmentUser
        {
            UserId = userId,
            FirstName = Guid.CreateVersion7().ToString(),
            LastName = Guid.CreateVersion7().ToString(),
            Email = $"{Guid.CreateVersion7()}modsen-software.com",
            IsActive = true
        };

        var approver = new EquipmentUser
        {
            UserId = approverId,
            FirstName = Guid.CreateVersion7().ToString(),
            LastName = Guid.CreateVersion7().ToString(),
            Email = $"{Guid.CreateVersion7()}modsen-software.com",
            Position = PortalRoleNames.HeadOfDepartment,
            IsActive = true
        };

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>
            {
                user, approver
            }.BuildMockDbSet().Object);

        _isEquipmentExistByIdDbQueryMock
            .Setup(x => x.ExecuteAsync(equipmentId, _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.ApproverDoesNotHavePermission);
    }

    [Fact]
    public async Task HandleWhenEquipmentAssignHasNoReturnDateShouldThrowEntityConflictException()
    {
        // Arrange
        var equipmentId = 3;
        const int userId = 5;
        const int approverId = 3;
        var issueDate = DateTime.UtcNow.Date;

        var request = new AssignUserToEquipmentRequest.Command(
            equipmentId,
            userId,
            approverId,
            issueDate);

        var user = new EquipmentUser
        {
            UserId = userId,
            FirstName = Guid.CreateVersion7().ToString(),
            LastName = Guid.CreateVersion7().ToString(),
            Email = $"{Guid.CreateVersion7()}modsen-software.com",
            DepartmentId = "Net",
            IsActive = true
        };

        var approver = new EquipmentUser
        {
            UserId = approverId,
            FirstName = Guid.CreateVersion7().ToString(),
            LastName = Guid.CreateVersion7().ToString(),
            Email = $"{Guid.CreateVersion7()}modsen-software.com",
            DepartmentId = "Net",
            Position = PortalRoleNames.HeadOfDepartment,
            IsActive = true
        };

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>
            {
                user, approver
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.EquipmentAssigns)
            .Returns(new List<EquipmentAssignEntity>
            {
                new()
                {
                    IssueDate = issueDate,
                    IsDeleted = false,
                    EquipmentId = equipmentId
                }
            }.BuildMockDbSet().Object);

        _isEquipmentExistByIdDbQueryMock
            .Setup(x => x.ExecuteAsync(equipmentId, _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityConflictException>()
            .WithMessage(PortalMessages.EquipmentAlreadyAssigned);
    }

    [Fact]
    public async Task HandleWhenEquipmentAssignsMaxReturnDateMoreThanRequestIssueDateShouldThrowEntityConflictException()
    {
        // Arrange
        var equipmentId = 3;
        const int userId = 5;
        const int approverId = 3;
        var issueDate = DateTime.UtcNow.Date;
        var returnDate = DateTime.UtcNow.Date.AddDays(+1);

        var request = new AssignUserToEquipmentRequest.Command(
            equipmentId,
            userId,
            approverId,
            issueDate);

        var user = new EquipmentUser
        {
            UserId = userId,
            FirstName = Guid.CreateVersion7().ToString(),
            LastName = Guid.CreateVersion7().ToString(),
            Email = $"{Guid.CreateVersion7()}modsen-software.com",
            IsActive = true
        };

        var approver = new EquipmentUser
        {
            UserId = approverId,
            FirstName = Guid.CreateVersion7().ToString(),
            LastName = Guid.CreateVersion7().ToString(),
            Email = $"{Guid.CreateVersion7()}modsen-software.com",
            Position = PortalRoleNames.Ceo,
            IsActive = true
        };

        _isEquipmentExistByIdDbQueryMock
            .Setup(x => x.ExecuteAsync(equipmentId, _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>
            {
                user, approver
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.EquipmentAssigns)
            .Returns(new List<EquipmentAssignEntity>
            {
                new()
                {
                    IsDeleted = false,
                    EquipmentId = equipmentId,
                    ReturnDate = returnDate
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityConflictException>()
            .WithMessage(PortalMessages.EquipmentAlreadyAssigned);
    }



    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public async Task HandleWhenCommandIsValidShouldAssignEquipment(int assignsCount)
    {
        // Arrange
        var equipmentId = 4;
        const int userId = 5;
        const int approverId = 3;
        var issueDate = DateTime.UtcNow.Date;

        var request = new AssignUserToEquipmentRequest.Command(
            equipmentId,
            userId,
            approverId,
            issueDate);

        var user = new EquipmentUser
        {
            UserId = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@modsen-software.com",
            IsActive = true
        };

        var approver = new EquipmentUser
        {
            UserId = approverId,
            FirstName = Guid.CreateVersion7().ToString(),
            LastName = Guid.CreateVersion7().ToString(),
            Email = $"{Guid.CreateVersion7()}modsen-software.com",
            Position = PortalRoleNames.Ceo,
            IsActive = true
        };

        var equipmentAssignsReturnDates = new DateTime?[assignsCount];
        for (var i = 0; i < assignsCount; i++)
        {
            equipmentAssignsReturnDates[i] = issueDate.AddDays(-i);
        }

        var createdAssignment = new EquipmentAssignEntity
        {
            Id = 1,
            AssignedToUserId = 5,
            IssueDate = issueDate,
            ApproverId = 3
        };

        var expectedDto = new EquipmentHolderDto
        {
            EquipmentHistoryId = 0,
            Id = userId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ApproverId = approverId,
            ApproverFirstName = approver.FirstName,
            ApproverLastName = approver.LastName,
            DepartmentId = user.DepartmentId,
            IssueDate = createdAssignment.IssueDate,
            ReturnDate = null
        };

        _isEquipmentExistByIdDbQueryMock
            .Setup(x => x.ExecuteAsync(equipmentId, _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>
            {
                user, approver
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.EquipmentAssigns)
            .Returns(new List<EquipmentAssignEntity>
            {
                createdAssignment
            }.BuildMockDbSet().Object);


        // Act
        var result = await _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }
}
