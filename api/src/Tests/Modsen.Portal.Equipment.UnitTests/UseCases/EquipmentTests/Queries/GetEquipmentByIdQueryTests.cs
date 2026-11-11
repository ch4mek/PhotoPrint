using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.Dto;
using Modsen.Portal.Equipment.Requests.Equipment.Requests;

namespace Modsen.Portal.Equipment.UnitTests.UseCases.EquipmentTests.Queries;

public class GetEquipmentByIdQueryTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IGetEquipmentByIdDbQuery> _getEquipmentByIdDbQueryMock = new();

    private readonly CancellationToken _cancellationToken;
    private readonly GetEquipmentByIdRequest _handler;

    public GetEquipmentByIdQueryTests()
    {
        _cancellationToken = CancellationToken.None;
        _handler = new GetEquipmentByIdRequest(
            _authenticatedUserContextMock.Object,
            _getEquipmentByIdDbQueryMock.Object);
    }

    [Fact]
    public async Task HandleWhenEquipmentDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var equipmentId = 1;
        var request = new GetEquipmentByIdRequest.Query(equipmentId);

        _getEquipmentByIdDbQueryMock.Setup(x => x.ExecuteAsync(
                equipmentId,
                _cancellationToken))
            .ReturnsAsync((EquipmentEntity)null);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Theory]
    [MemberData(nameof(GetDepartmentsTestData))]
    public async Task HandleWhenUserIsHeadOfDepartmentAndEquipmentIsAssignedToUserOfOtherDepartmentShouldThrowInvalidDataAppException(
        string userDepartmentId,
        string? holderDepartmentId)
    {
        // Arrange
        var equipmentId = 2;
        var request = new GetEquipmentByIdRequest.Query(equipmentId);

        var equipment = new EquipmentEntity
        {
            Id = equipmentId,
            Characteristics = "Characteristics",
            Name = "Name",
            PurchasePlace = "PurchasePlace",
            SerialNumber = "SerialNumber",
            BrokenStatus = EquipmentBrokenStatus.None,
            Approver = new EquipmentUser
            {
                UserId = 22,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@modsen-software.com"
            },
            Assigns =
            [
                new EquipmentAssignEntity
                {
                    AssignedToUserId = 5,
                    EquipmentId = request.Id,
                    IssueDate = DateTime.UtcNow.Date.AddDays(-5),
                    User = new EquipmentUser
                    {
                        UserId = 5,
                        FirstName = "Lorem",
                        LastName = "Ipsum",
                        Email = "lorem.ipsum@modsen-software.com",
                        DepartmentId = holderDepartmentId!
                    }
                }
            ]
        };

        _getEquipmentByIdDbQueryMock.Setup(x => x.ExecuteAsync(
                equipmentId,
                _cancellationToken))
            .ReturnsAsync(equipment);
        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns(userDepartmentId);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentAndEquipmentIsNotAssignedShouldThrowActionNotAllowedException()
    {
        // Arrange
        var equipmentId = 3;
        var request = new GetEquipmentByIdRequest.Query(equipmentId);

        var equipment = new EquipmentEntity
        {
            Id = equipmentId,
            Characteristics = "Characteristics",
            Name = "Name",
            BrokenStatus = EquipmentBrokenStatus.Broken,
            PurchasePlace = "PurchasePlace",
            SerialNumber = "SerialNumber",
            Approver = new EquipmentUser
            {
                UserId = 22,
                FirstName = "Lorem",
                LastName = "Ipsum",
                Email = "lorem.ipsum@modsen-software.com"
            }
        };

        _getEquipmentByIdDbQueryMock.Setup(x => x.ExecuteAsync(
                equipmentId,
                _cancellationToken))
            .ReturnsAsync(equipment);
        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns("Net");

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenEquipmentWithHolderShouldReturnDtoWithMappedHolder()
    {
        // Arrange
        var equipmentId = 4;
        var request = new GetEquipmentByIdRequest.Query(equipmentId);

        var equipment = new EquipmentEntity
        {
            Id = equipmentId,
            Characteristics = "Characteristics",
            Name = "Name",
            BrokenStatus = EquipmentBrokenStatus.Broken,
            PurchasePlace = "PurchasePlace",
            SerialNumber = "SerialNumber",
            JiraTask = "qwe",
            Approver = new EquipmentUser
            {
                UserId = 22,
                FirstName = "Lorem",
                LastName = "Ipsum",
                Email = "lorem.ipsum@modsen-software.com"
            },
            Assigns =
            [
                new EquipmentAssignEntity
                {
                    AssignedToUserId = 5,
                    EquipmentId = request.Id,
                    IssueDate = DateTime.UtcNow.Date.AddDays(-5),
                    User = new EquipmentUser
                    {
                        UserId = 5,
                        FirstName = "Lorem",
                        LastName = "Ipsum",
                        Email = "lorem.ipsum@modsen-software.com"
                    },
                    ApproverId = 3,
                    Approver = new EquipmentUser
                    {
                        UserId = 3,
                        FirstName = Guid.CreateVersion7().ToAlphabetic(),
                        LastName = Guid.CreateVersion7().ToAlphabetic(),
                        Email = $"{Guid.CreateVersion7().ToAlphabetic()}modsen-software.com"
                    }

                }
            ]
        };

        var activeHolderDto = new EquipmentHolderDto
        {
            Id = equipment.Assigns.ToList()[0].User!.UserId,
            FirstName = equipment.Assigns.ToList()[0].User!.FirstName,
            LastName = equipment.Assigns.ToList()[0].User!.LastName,
            IssueDate = equipment.Assigns.ToList()[0].IssueDate,
            ApproverId = equipment.Assigns.ToList()[0].ApproverId,
            ApproverFirstName = equipment.Assigns.ToList()[0].Approver.FirstName,
            ApproverLastName = equipment.Assigns.ToList()[0].Approver.LastName
        };

        var expectedEquipmentDto = new EquipmentCoreDto
        {
            Id = equipment.Id,
            Characteristics = equipment.Characteristics,
            Location = equipment.Location,
            BrokenStatus = EquipmentBrokenStatus.Broken,
            Name = equipment.Name,
            PurchasePlace = equipment.PurchasePlace,
            SerialNumber = equipment.SerialNumber,
            ActiveHolder = activeHolderDto,
            JiraTask = "qwe",
            Approver = new ApproverDto
            {
                Id = equipment.Approver.UserId,
                FirstName = equipment.Approver.FirstName,
                LastName = equipment.Approver.LastName
            }
        };

        _getEquipmentByIdDbQueryMock.Setup(x => x.ExecuteAsync(
                equipmentId,
                _cancellationToken))
            .ReturnsAsync(equipment);
        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);

        // Act
        var result = await _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(
            expectedEquipmentDto,
            options => options.Excluding(x => x.ActiveHolder));
        result.ActiveHolder.Should().NotBeNull();
    }

    [Fact]
    public async Task HandleWhenEquipmentWithoutHolderShouldReturnDtoWithoutHolder()
    {
        // Arrange
        var equipmentId = 5;
        var request = new GetEquipmentByIdRequest.Query(equipmentId);

        var equipment = new EquipmentEntity
        {
            Id = equipmentId,
            Characteristics = "Characteristics",
            Name = "Name",
            BrokenStatus = EquipmentBrokenStatus.Renovation,
            PurchasePlace = "PurchasePlace",
            SerialNumber = "SerialNumber",
            JiraTask = "qwe",
            Approver = new EquipmentUser
            {
                UserId = 22,
                FirstName = "Lorem",
                LastName = "Ipsum",
                Email = "lorem.ipsum@modsen-software.com"
            }
        };

        var expectedEquipmentDto = new EquipmentCoreDto
        {
            Id = equipment.Id,
            Characteristics = equipment.Characteristics,
            Location = equipment.Location,
            BrokenStatus = EquipmentBrokenStatus.Renovation,
            Name = equipment.Name,
            PurchasePlace = equipment.PurchasePlace,
            SerialNumber = equipment.SerialNumber,
            JiraTask = "qwe",
            Approver = new ApproverDto
            {
                Id = equipment.Approver.UserId,
                FirstName = equipment.Approver.FirstName,
                LastName = equipment.Approver.LastName
            }
        };

        _getEquipmentByIdDbQueryMock.Setup(x => x.ExecuteAsync(
                equipmentId,
                _cancellationToken))
            .ReturnsAsync(equipment);
        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Cto]);

        // Act
        var result = await _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedEquipmentDto);
    }

    public static IEnumerable<object?[]> GetDepartmentsTestData()
    {
        yield return ["Java", "DevOps"];
        yield return ["Net", "Js"];
        yield return ["Js", null];
    }
}
