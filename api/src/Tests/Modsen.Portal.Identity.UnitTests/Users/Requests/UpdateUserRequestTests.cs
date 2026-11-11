using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Core;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Core.Validators;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class UpdateUserRequestTests
{
    private readonly Mock<ICommunicationBus> _outboxContractMock = new();
    private readonly Mock<IHexColorGenerator> _hexColorGeneratorMock = new();
    private readonly Mock<IAuthenticationTokenContext> _authenticatedTokenContext = new();
    private readonly Mock<ICanUserFieldsBeUpdatedBusinessRule> _canUserBeUpdatedBusinessRuleMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ITeamBoardContract> _teamBoardContract = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IUserLoader> _userLoaderMock = new();
    private readonly Mock<IRolesCache> _rolesCacheMock = new();
    private readonly Mock<IUpdateUserValidator> _updateUserValidatorMock = new();
    private readonly Mock<IUserDepartmentHistoryComposer> _userDepartmentHistoryComposerMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly UpdateUserRequest _commandHandler;
    public UpdateUserRequestTests()
    {
        _commandHandler = new UpdateUserRequest(
            _hexColorGeneratorMock.Object,
            _outboxContractMock.Object,
            _authenticatedTokenContext.Object,
            _canUserBeUpdatedBusinessRuleMock.Object,
            _authenticatedUserContextMock.Object,
            _teamBoardContract.Object,
            _identityDbContextMock.Object,
            _userLoaderMock.Object,
            _rolesCacheMock.Object,
            _updateUserValidatorMock.Object,
            _userDepartmentHistoryComposerMock.Object);
    }

    [Fact]
    public async Task HandleWhenDataIsCorrectReturnUpdatedUser()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: "#ffffff",
            CountryId: 1,
            RegisteredLocationId: PortalSharedConstants.DefaultRegisteredLocationId,
            CurrentLocation: "Minsk",
            Email: null,
            PortalRoleNames.Ceo,
            LocationType.Minsk,
            null,
            ResourceGroupId: null,
            [PortalRoleNames.Ceo],
            null,
            HireDate: null);

        var userDto = new UserDto
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "example@email.com",
            LastName = "LastName",
            RoleNames = new List<string> { PortalRoleNames.Ceo },
            DepartmentId = null,
            PositionOrderWeight = 100,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var userEntity = new User
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "example@email.com",
            LastName = "LastName",
            DepartmentId = null,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            IsActive = true,
            UserRoles = [new UserRole { Role = new Role { Name = requestModel.Position, PublicName = "Test" } }]
        };

        var updatedUserDto = new UserDto
        {
            Id = userDto.Id,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            FullName = $"{userDto.LastName} {userDto.FirstName}",
            Email = userDto.Email,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            Color = requestModel.Color,
            CountryId = requestModel.CountryId,
            Location = requestModel.Location,
        };

        var ceoRoleOrderWeight = 1000;
        var role = new RoleCacheRecord
        {
            Name = requestModel.Position,
            OrderWeight = ceoRoleOrderWeight,
        };

        _rolesCacheMock
            .Setup(x => x.GetAllRoles(_cancellationToken))
            .ReturnsAsync([role]);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { userEntity }.BuildMockDbSet().Object);
        _identityDbContextMock
            .Setup(x => x.ResourceGroups)
            .Returns(new List<ResourceGroup>().BuildMockDbSet().Object);

        _userLoaderMock
            .SetupSequence(x => x.LoadAsync(userDto.Id, _cancellationToken))
            .ReturnsAsync(userDto)
            .ReturnsAsync(updatedUserDto);

        var model = new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel);

        _canUserBeUpdatedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(model, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _teamBoardContract.Setup(x => x.GetCountries(_cancellationToken))
            .ReturnsAsync([new CountryOutput { Id = 1, Name = "Belarus" }]);

        // Act
        var result = await _commandHandler.Handle(
            requestModel,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(updatedUserDto);

        _userLoaderMock.Verify(
            x => x.LoadAsync(requestModel.Id,_cancellationToken), Times.Exactly(2));

        _outboxContractMock.Verify(
            x => x.PublishAsync(
                It.Is<UserUpdatedEvent>(u =>
                u.UserId == updatedUserDto.Id &&
                u.LastName == updatedUserDto.LastName &&
                u.FirstName == updatedUserDto.FirstName &&
                u.DepartmentId == updatedUserDto.DepartmentId &&
                u.Email == updatedUserDto.Email &&
                u.CountryId == updatedUserDto.CountryId &&
                u.Color == updatedUserDto.Color &&
                u.AvatarFileId == updatedUserDto.AvatarFileId &&
                u.Position == updatedUserDto.Position &&
                u.Location == updatedUserDto.Location &&
                u.ResourceGroupId == updatedUserDto.ResourceGroupId &&
                u.RegisteredLocationId == updatedUserDto.RegisteredLocationId),
                _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistThrowNotFoundException()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: "#ffffff",
            CountryId: 1,
            RegisteredLocationId: PortalSharedConstants.DefaultRegisteredLocationId,
            CurrentLocation: "Minsk",
            Email: null,
            PortalRoleNames.Employee,
            LocationType.Minsk,
            "Js",
            ResourceGroupId: null,
            [PortalRoleNames.HeadOfDepartment],
            "Js",
            HireDate: null);

        _userLoaderMock
            .Setup(x => x.LoadAsync(requestModel.Id, _cancellationToken))
            .Throws<EntityNotFoundException>();

        _teamBoardContract.Setup(x => x.GetCountries(_cancellationToken))
            .ReturnsAsync([new CountryOutput { Id = 1, Name = "Belarus" }]);

        // Act
        var result = () => _commandHandler.Handle(
            requestModel,
            _cancellationToken);

       // Assert
       await result.Should().ThrowAsync<EntityNotFoundException>();

        _userLoaderMock.Verify(
            x => x.LoadAsync(requestModel.Id, _cancellationToken), Times.Once);
        _outboxContractMock.Verify(
            x => x.PublishAsync(
                It.IsAny<UserUpdatedEvent>(),
                _cancellationToken),
            Times.Never);
    }

    [Fact]
    public async Task HandleWhenCurrentUserAuthedAsSystemAdministratorAndTryToSetDepartmentToNullShouldNotCallDepartmentIdBusinessRule()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: "#ffffff",
            CountryId: 1,
            RegisteredLocationId: PortalSharedConstants.DefaultRegisteredLocationId,
            CurrentLocation: null,
            Email: null,
            Position: PortalRoleNames.Ceo,
            Location: LocationType.Minsk,
            DepartmentId: null,
            ResourceGroupId: null,
            CurrentUserRole: [PortalRoleNames.Ceo],
            CurrentUserDepartmentId: null,
            HireDate: null);

        var userDto = new UserDto()
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "example@email.com",
            LastName = "LastName",
            RoleNames = new List<string> { PortalRoleNames.Ceo },
            DepartmentId = null,
            PositionOrderWeight = 100,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var updatedUserDto = new UserDto
        {
            Id = userDto.Id,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            FullName = $"{userDto.LastName} {userDto.FirstName}",
            Email = userDto.Email,
            DepartmentId = userDto.DepartmentId,
            RoleNames = [userDto.Position!],
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var userEntity = new User
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "example@email.com",
            LastName = "LastName",
            DepartmentId = null,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            IsActive = true,
            UserRoles = [new UserRole { Role = new Role { Name = requestModel.Position, PublicName = "Test" } }]
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.SystemAdministrator]);

        var ceoRoleOrderWeight = 1000;
        var role = new RoleCacheRecord
        {
            Name = requestModel.Position,
            OrderWeight = ceoRoleOrderWeight,
        };

        _rolesCacheMock
            .Setup(x => x.GetAllRoles(_cancellationToken))
            .ReturnsAsync([role]);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { userEntity }.BuildMockDbSet().Object);
        _identityDbContextMock
            .Setup(x => x.ResourceGroups)
            .Returns(new List<ResourceGroup>().BuildMockDbSet().Object);

        _userLoaderMock
            .SetupSequence(x => x.LoadAsync(userDto.Id, _cancellationToken))
            .ReturnsAsync(userDto)
            .ReturnsAsync(updatedUserDto);

        var model = new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel);

        _canUserBeUpdatedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(model, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _teamBoardContract.Setup(x => x.GetCountries(_cancellationToken))
            .ReturnsAsync([new CountryOutput { Id = 1, Name = "Belarus" }]);

        // Act
        var result = () => _commandHandler.Handle(
            requestModel,
            _cancellationToken);

        // Assert
        await result.Should().NotThrowAsync();

        _userLoaderMock.Verify(
            x => x.LoadAsync(requestModel.Id, _cancellationToken), Times.Exactly(2));

        _outboxContractMock.Verify(
            x => x.PublishAsync(
                It.Is<UserUpdatedEvent>(u =>
                u.UserId == updatedUserDto.Id &&
                u.LastName == updatedUserDto.LastName &&
                u.FirstName == updatedUserDto.FirstName &&
                u.DepartmentId == updatedUserDto.DepartmentId &&
                u.Email == updatedUserDto.Email &&
                u.CountryId == updatedUserDto.CountryId &&
                u.Color == updatedUserDto.Color &&
                u.AvatarFileId == updatedUserDto.AvatarFileId &&
                u.Position == updatedUserDto.Position &&
                u.Location == updatedUserDto.Location &&
                u.ResourceGroupId == updatedUserDto.ResourceGroupId &&
                u.RegisteredLocationId == updatedUserDto.RegisteredLocationId),
                _cancellationToken),
            Times.Once);
    }
}
