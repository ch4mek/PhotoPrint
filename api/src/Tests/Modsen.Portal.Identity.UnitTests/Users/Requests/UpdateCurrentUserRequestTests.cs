using Bogus;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.IdentityPortalSettings;
using Modsen.Portal.Identity.Requests.Users.Core;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Modsen.Portal.Storage.Contract;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class UpdateCurrentUserRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserMock = new Mock<AuthenticatedUserContext>
        { CallBase = true }.As<IAuthenticatedUserContext>();
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();
    private readonly Mock<IStorageContract> _storageContractMock = new();
    private readonly Mock<IAuthenticationTokenContext> _authenticatedTokenContext= new();
    private readonly Mock<ICanRoleBeOccupiedBusinessRule> _canRoleBeOccupiedBusinessRuleMock = new();
    private readonly Mock<ICanRoleHaveDepartmentBusinessRule> _canRoleHaveDepartmentBusinessRuleMock = new();
    private readonly Mock<IIdentityPortalSettingsAccessor> _settingsAccessor = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IUserLoader> _userLoaderMock = new();
    private readonly Mock<IRolesCache> _rolesCacheMock = new();
    private readonly Mock<IUserDepartmentHistoryComposer> _userDepartmentHistoryComposerMock = new();

    private readonly Faker<UpdateCurrentUserRequest.Command> _createUserCommandFaker = new();
    private readonly Faker<UserDto> _userDtoFaker = new();
    private readonly Faker<User> _userFaker = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly UpdateCurrentUserRequest _commandHandler;

    public UpdateCurrentUserRequestTests()
    {
        _commandHandler = new UpdateCurrentUserRequest(
            _authenticatedUserMock.Object,
            _communicationBusMock.Object,
            _storageContractMock.Object,
            _authenticatedTokenContext.Object,
            _canRoleBeOccupiedBusinessRuleMock.Object,
            _canRoleHaveDepartmentBusinessRuleMock.Object,
            _identityDbContextMock.Object,
            _userLoaderMock.Object,
            _rolesCacheMock.Object,
            _settingsAccessor.Object,
            _userDepartmentHistoryComposerMock.Object);

        _createUserCommandFaker.CustomInstantiator(faker => new UpdateCurrentUserRequest.Command(
            TimeZone: "Poland",
            AvatarColor: null,
            AvatarFileId: null,
            DepartmentId: null,
            Location: faker.PickRandom<LocationType>()));

        _userFaker.CustomInstantiator(_ => new User()
        {
            FirstName = "FirstName",
            LastName = "test",
            Id = 1,
            Email = "LastName@modsen-software.com",
            UserRoles = new List<UserRole>
            { { new() { Role = new Role { Name = PortalRoleNames.Employee, PublicName = "Test" } } } }
        });

        _userDtoFaker.CustomInstantiator(faker => new UserDto
        {
            LastName = "LastName",
            FirstName = "UpdatedFirstName",
            FullName = "LastName UpdatedFirstName",
            Email = "UpdatedLastName@modsen-software.com",
            Location = faker.PickRandom<LocationType>(),
            PositionOrderWeight = 100,
        });

        _settingsAccessor
            .Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdentityPortalSettingsDto
            {
                MaxCeoCount = 1,
                MaxCtoCount = 1,
                MaxHeadOfSpecificDepartmentCount = 10,
                DefaultTimeZone = "BY",
                DefaultColorHex = "#BDBDBD",
                EmailDomain = "@modsen-software.com"
            });
    }

    [Fact]
    public async Task UpdateCurrentUserWhenUserExistsReturnSuccessResult()
    {
        //Arrange
        var requestModel = _createUserCommandFaker.Generate();

        var userDto = _userDtoFaker.Generate();
        var user = _userFaker.Generate();
        var updatedIdentityUser = _userFaker.Clone()
            .RuleFor(x => x.LastName, _ => userDto.LastName)
            .Generate();

        _authenticatedUserMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRequiredUserId()).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);

        _canRoleHaveDepartmentBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _token))
            .ReturnsAsync(BusinessRuleResult.Success);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);

        _userLoaderMock
            .Setup(x => x.LoadAsync(1, _token))
            .ReturnsAsync(userDto);

        //Act
        var result = await _commandHandler.Handle(requestModel, _token);

        //Assert
        result.Should().NotBeNull();
        result.LastName.Should().NotBe(user.LastName);
    }

    [Fact]
    public async Task UpdateCurrentUserWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        //Arrange
        var requestModel = _createUserCommandFaker.Generate();

        _authenticatedUserMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRequiredUserId()).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>().BuildMockDbSet().Object);

        //Act
        var result = async () => { await _commandHandler.Handle(requestModel, _token); };

        //Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenUserWithProvidedRoleCannotHaveProvidedDepartmentShouldThrowInvalidDataAppException()
    {
        // Arrange
        var requestModel = _createUserCommandFaker.Generate();
        var user = _userFaker.Generate();
        const string failReason = "FailReason";

        _authenticatedUserMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRequiredUserId()).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);

        _canRoleHaveDepartmentBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _token))
            .ReturnsAsync(BusinessRuleResult.Fail(failReason));

        // Act
        var result = () => _commandHandler.Handle(
            requestModel,
            CancellationToken.None);

        // Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(failReason);
    }

    [Fact]
    public async Task HandleWhenHeadAddedToDepartmentWhichAlreadyHasHeadShouldThrowActionNotAllowedException()
    {
        // Arrange
        var requestModel = _createUserCommandFaker.RuleFor(x => x.DepartmentId, "Net").Generate();

        var user = _userFaker.Generate();
        user.UserRoles.First().Role!.Name = PortalRoleNames.HeadOfDepartment;
        user.DepartmentId = null;

        const string failReason = "FailReason";

        _authenticatedUserMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRequiredUserId()).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);

        _canRoleHaveDepartmentBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _token))
            .ReturnsAsync(BusinessRuleResult.Success);
        _canRoleBeOccupiedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleBeOccupiedBusinessRule.Request>(),
                _token))
            .ReturnsAsync(BusinessRuleResult.Fail(failReason));

        // Act
        var result = () => _commandHandler.Handle(
            requestModel,
            CancellationToken.None);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(failReason);
    }

    [Fact]
    public async Task HandleWhenUserChangeDepartmentShouldThrowActionNotAllowedException()
    {
        // Arrange
        var requestModel = _createUserCommandFaker.RuleFor(x => x.DepartmentId, "Net").Generate();

        var user = _userFaker.Generate();
        user.DepartmentId = "Js";

        _authenticatedUserMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRequiredUserId()).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);

        _canRoleHaveDepartmentBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _token))
            .ReturnsAsync(BusinessRuleResult.Success);
        _canRoleBeOccupiedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleBeOccupiedBusinessRule.Request>(),
                _token))
            .ReturnsAsync(BusinessRuleResult.Success);

        // Act
        var result = () => _commandHandler.Handle(
            requestModel,
            CancellationToken.None);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task UpdateCurrentUserWhenUserExistsAndAvatarColorIsNullReturnSuccessResult()
    {
        //Arrange
        const string? currentColor = null;
        const string? newColor = "#ffffff";

        var requestModel = _createUserCommandFaker.Clone()
            .RuleFor(x => x.AvatarColor, _ => newColor);

        var userDto = _userDtoFaker.Clone()
            .RuleFor(x => x.AvatarColor, newColor)
            .Generate();
        var user = _userFaker.Clone()
            .RuleFor(x => x.AvatarColor, _ => currentColor)
            .Generate();
        var updatedIdentityUser = _userFaker.Clone()
            .RuleFor(x => x.AvatarColor, _ => newColor)
            .Generate();

        _authenticatedUserMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRequiredUserId()).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);

        _identityDbContextMock
        .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);

        _userLoaderMock
            .Setup(x => x.LoadAsync(1, _token))
            .ReturnsAsync(userDto);

        _canRoleHaveDepartmentBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _token))
            .ReturnsAsync(BusinessRuleResult.Success);

        //Act
        var result = await _commandHandler.Handle(requestModel, _token);

        //Assert
        result.Should().NotBeNull();
        result.AvatarColor.Should().Be(newColor);
    }

    [Fact]
    public async Task UpdateCurrentUserWhenUserExistsAndDepartmentWillBeUpdatedReturnSuccessResultAndAddTokenToCache()
    {
        //Arrange
        const string? currentDepartmentId = null;
        const string? newDepartmentId = "Js";

        var requestModel = _createUserCommandFaker.Clone()
            .RuleFor(x => x.DepartmentId, _ => newDepartmentId);

        var userDto = _userDtoFaker.Clone()
            .RuleFor(x => x.DepartmentId, newDepartmentId)
            .Generate();
        var user = _userFaker.Clone()
            .RuleFor(x => x.DepartmentId, _ => currentDepartmentId)
            .Generate();
        var updatedIdentityUser = _userFaker.Clone()
            .RuleFor(x => x.DepartmentId, _ => newDepartmentId)
            .Generate();

        _authenticatedUserMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRequiredUserId()).Returns(1);
        _authenticatedUserMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);

        _identityDbContextMock
        .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);
        _userLoaderMock
            .Setup(x => x.LoadAsync(1, _token))
            .ReturnsAsync(userDto);

        _canRoleHaveDepartmentBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _token))
            .ReturnsAsync(BusinessRuleResult.Success);

        //Act
        var result = await _commandHandler.Handle(requestModel, _token);

        //Assert
        result.Should().NotBeNull();
        result.DepartmentId.Should().Be("Js");
    }
}
