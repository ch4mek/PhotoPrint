using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.IdentityPortalSettings;
using Modsen.Portal.Identity.Requests.Users.Core;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class CreateUserRequestTests
{
    private readonly Mock<ICommunicationBus> _outboxContractMock = new();
    private readonly Mock<IHexColorGenerator> _hexColorGeneratorMock = new();
    private readonly Mock<ICanCurrentUserCreateUserWithRoleBusinessRule> _canUserWithRoleBeCreatedBusinessRuleMock = new();
    private readonly Mock<ICanRoleBeOccupiedBusinessRule> _canRoleBeOccupiedBusinessRuleMock = new();
    private readonly Mock<ICanRoleHaveDepartmentBusinessRule> _canRoleHaveDepartmentBusinessRule = new();
    private readonly Mock<ICanUserWithResourceGroupBeCreatedBusinessRule> _canUserWithResourceGroupBeCreatedBusinessRule = new();
    private readonly Mock<IIdentityPortalSettingsAccessor> _settingsAccessor = new();
    private readonly Mock<ITeamBoardContract> _teamBoardContract = new();
    private readonly Mock<IRolesCache> _rolesCacheMock = new();
    private readonly Mock<IUserLoader> _userLoaderMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IUserDepartmentHistoryComposer> _userDepartmentHistoryComposerMock = new();

    private readonly Faker<CreateUserRequest.Command> _createUserCommandFaker = new Faker<CreateUserRequest.Command>()
        .CustomInstantiator(x => new CreateUserRequest.Command(
            "Test name",
            "Test surname",
            "Тестовое имя",
            "Тестовая фамилия",
            "Тестовое отчество",
            "#fff",
            CountryId: 1,
            CurrentLocation: "Minsk",
            RegisteredLocationId: PortalSharedConstants.DefaultRegisteredLocationId,
            $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            null, null, null, null, null,
            PortalRoleNames.Employee,
            LocationType.Minsk,
            "Js",
            1,
            MilitaryServiceStatus.Fit,
            null,
            ContractDateFrom: DateOnly.FromDateTime(DateTime.Today),
            ContractDateTo: DateOnly.FromDateTime(DateTime.Today).AddYears(1),
            HireDate: DateOnly.FromDateTime(DateTime.Today),
            DateOfBirth: new DateOnly(2000, 2, 2))
        );

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly CreateUserRequest _commandHandler;

    public CreateUserRequestTests()
    {
        _commandHandler = new CreateUserRequest(
            _hexColorGeneratorMock.Object,
            _outboxContractMock.Object,
            _canUserWithRoleBeCreatedBusinessRuleMock.Object,
            _canRoleBeOccupiedBusinessRuleMock.Object,
            _canRoleHaveDepartmentBusinessRule.Object,
            _canUserWithResourceGroupBeCreatedBusinessRule.Object,
            _settingsAccessor.Object,
            _teamBoardContract.Object,
            _identityDbContextMock.Object,
            _rolesCacheMock.Object,
            _userLoaderMock.Object,
            _userDepartmentHistoryComposerMock.Object);

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

        _identityDbContextMock
            .Setup(x => x.Departments)
            .Returns(new List<Department>() { new()
            {
                Id = "Js",
                FullName = "JavaScript",
                ShortName = "Js"
            } }.BuildMockDbSet().Object);
    }

    [Fact]
    public async Task HandleWhenUserWithProvidedRoleCannotHaveProvidedDepartmentShouldThrowInvalidDataAppException()
    {
        // Arrange
        var requestModel = _createUserCommandFaker.Generate();
        const string failReason = "FailReason";

        _canRoleHaveDepartmentBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail(failReason));

        // Act
        var result = () => _commandHandler.Handle(
            requestModel,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(failReason);
    }

    [Theory]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task HandleWhenUserIsChiefOrHeadWithResourceGroupShouldThrowActionNotAllowedException(string role)
    {
        // Arrange
        var requestModel = _createUserCommandFaker.RuleFor(u => u.Position, role).Generate();

        _canRoleHaveDepartmentBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        // Act
        var result = () => _commandHandler.Handle(
            requestModel,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenUserWithResourceGroupCanNotBeCreatedShouldThrowActionNotAllowedException()
    {
        // Arrange
        var requestModel = _createUserCommandFaker.RuleFor(u => u.Position, PortalRoleNames.Employee).Generate();
        const string failReason = "FailReason";

        _canRoleHaveDepartmentBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _canUserWithResourceGroupBeCreatedBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanUserWithResourceGroupBeCreatedBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail(failReason));

        // Act
        var result = () => _commandHandler.Handle(
            requestModel,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(failReason);
    }

    [Fact]
    public async Task HandleWhenCurrentUserCannotCreateUserWithSpecifiedRoleShouldThrowActionNotAllowedException()
    {
        // Arrange
        var requestModel = _createUserCommandFaker.RuleFor(u => u.ResourceGroupId, (int?)null).Generate();
        const string failReason = "FailReason";

        _canRoleHaveDepartmentBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _canUserWithResourceGroupBeCreatedBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanUserWithResourceGroupBeCreatedBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _canUserWithRoleBeCreatedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanCurrentUserCreateUserWithRoleBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail(failReason));

        // Act
        var result = () => _commandHandler.Handle(
            requestModel,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(failReason);
    }

    [Fact]
    public async Task HandleWhenUserWithThisEmailExistShouldThrowEntityConflictException()
    {
        // Arrange
        var requestModel = _createUserCommandFaker.RuleFor(u => u.ResourceGroupId, (int?)null).Generate();

        _canRoleHaveDepartmentBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _canUserWithResourceGroupBeCreatedBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanUserWithResourceGroupBeCreatedBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _canUserWithRoleBeCreatedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanCurrentUserCreateUserWithRoleBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _canRoleBeOccupiedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleBeOccupiedBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        var country = new CountryOutput()
        {
            Id = requestModel.CountryId ?? 0,
            Name = "country"
        };

        _teamBoardContract
            .Setup(x => x.GetCountries(_cancellationToken))
            .ReturnsAsync([country]);
        _teamBoardContract
            .Setup(x => x.GetCountries(_cancellationToken))
            .ReturnsAsync([country]);

        var user = new User
        {
            Id = 1,
            NormalizedEmail = requestModel.Email.Normalize().ToUpperInvariant(),
            FirstName = "test",
            LastName = "test"
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);

        var createdGroup = new ResourceGroup {Id = 1, Name = "RG_TEST" , DepartmentId = "Net"};

        var entityEntry = new Mock<EntityEntry<ResourceGroup>>(new InternalEntityEntry(
            new Mock<IStateManager>().Object,
            new RuntimeEntityType("T", typeof(int), false, null, null, null, ChangeTrackingStrategy.Snapshot, null,
                false, null, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
            createdGroup));

        entityEntry
            .Setup(e => e.Entity)
            .Returns(createdGroup);

        _identityDbContextMock
            .Setup(x => x.ResourceGroups.Add(It.IsAny<ResourceGroup>()))
            .Returns(entityEntry.Object);

        var createdUser = new User { Id = 1,
            FirstName = "test",
            LastName = "test"
        };

        var userEntity = new Mock<EntityEntry<User>>(new InternalEntityEntry(
            new Mock<IStateManager>().Object,
            new RuntimeEntityType("T", typeof(int), false, null, null, null, ChangeTrackingStrategy.Snapshot, null,
                false, null, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
            createdUser));

        userEntity
            .Setup(e => e.Entity)
            .Returns(createdUser);

        _identityDbContextMock
            .Setup(x => x.Users.Add(It.IsAny<User>()))
            .Returns(userEntity.Object);

        _rolesCacheMock
            .Setup(x => x.GetAllRoles(_cancellationToken))
            .ReturnsAsync([
                new() { Id = 1, Name = requestModel.Position, }
            ]);

        var userDto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.LastName} {user.FirstName}",
            Email = user.Email,
            DepartmentId = user.DepartmentId,
            RoleNames = [requestModel.Position],
            Location = requestModel.Location,
            PositionOrderWeight = 100,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(user.Id, _cancellationToken))
            .ReturnsAsync(userDto);

        // Act
        var result = () => _commandHandler.Handle(
            requestModel,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityConflictException>()
            .WithMessage($"Email - {requestModel.Email} is already used");
    }

    [Fact]
    public async Task HandleWhenRoleIsOccupiedShouldThrowActionNotAllowedException()
    {
        // Arrange
        var requestModel = _createUserCommandFaker.RuleFor(u => u.ResourceGroupId, (int?)null).Generate();
        var failReason = "Role is occupied";

        _canRoleHaveDepartmentBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _canUserWithResourceGroupBeCreatedBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanUserWithResourceGroupBeCreatedBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _canUserWithRoleBeCreatedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanCurrentUserCreateUserWithRoleBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>().BuildMockDbSet().Object);

        _canRoleBeOccupiedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleBeOccupiedBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail(failReason));

        // Act
        var result = () => _commandHandler.Handle(
            requestModel,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(failReason);
    }

    [Fact]
    public async Task HandleWhenPhoneOrEmailsAreAlreadyExistsThrowEntityConflictException()
    {
        // Arrange
        var requestModel = _createUserCommandFaker.RuleFor(u => u.ResourceGroupId, (int?)null).Generate();
        var failReason = "already exists";

        _canRoleHaveDepartmentBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _canUserWithResourceGroupBeCreatedBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanUserWithResourceGroupBeCreatedBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _canUserWithRoleBeCreatedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanCurrentUserCreateUserWithRoleBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>().BuildMockDbSet().Object);

        _canRoleBeOccupiedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleBeOccupiedBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _teamBoardContract
            .Setup(x => x.ArePersonalPhoneAndEmailsUniqueAsync(requestModel.PersonalEmail,
                requestModel.PersonalEmailSecond, requestModel.PersonalPhone, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail(failReason));

        _teamBoardContract.Setup(x => x.GetCountries(_cancellationToken))
            .ReturnsAsync([new CountryOutput { Id = 1, Name = "Belarus" }]);

        // Act
        var result = () => _commandHandler.Handle(
            requestModel,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityConflictException>()
            .WithMessage(failReason);
    }

    [Fact]
    public async Task HandleWhenDataIsCorrectReturnCreatedUser()
    {
        // Arrange
        var requestModel = new CreateUserRequest.Command(
            "Test name",
            "Test surname",
            "Тестовое имя",
            "Тестовая фамилия",
            "Тестовое отчество",
            "#fff",
            CountryId: 1,
            CurrentLocation: "Minsk",
            RegisteredLocationId: PortalSharedConstants.DefaultRegisteredLocationId,
            $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            null, null, null, null, null,
            PortalRoleNames.Employee,
            LocationType.Minsk,
            "Js",
            1,
            MilitaryServiceStatus.Fit,
            null,
            ContractDateFrom: DateOnly.FromDateTime(DateTime.Today),
            ContractDateTo: DateOnly.FromDateTime(DateTime.Today).AddYears(1),
            HireDate: DateOnly.FromDateTime(DateTime.Today),
            DateOfBirth: new DateOnly(2000, 2, 2));

        var user = new UserDto()
        {
            Id = 11,
            FirstName = requestModel.FirstName,
            Email = requestModel.Email,
            LastName = requestModel.LastName,
            RoleNames = [requestModel.Position],
            DepartmentId = requestModel.DepartmentId,
            PositionOrderWeight = 100,
            Location = requestModel.Location,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var userDto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.LastName} {user.FirstName}",
            Email = user.Email,
            DepartmentId = user.DepartmentId,
            RoleNames = [requestModel.Position],
            Location = requestModel.Location,
            PositionOrderWeight = 100,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var createdUser = new User
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DepartmentId = user.DepartmentId,
            Location = requestModel.Location,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var entityEntry = new Mock<EntityEntry<User>>(new InternalEntityEntry(
            new Mock<IStateManager>().Object,
            new RuntimeEntityType("T", typeof(int), false, null, null, null, ChangeTrackingStrategy.Snapshot, null,
                false, null, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
            createdUser));

        entityEntry.Setup(e => e.Entity)
            .Returns(createdUser);

        var userCreatedNotification = new UserCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DepartmentId = user.DepartmentId,
            Color = user.Color,
            AvatarFileId = user.AvatarFileId,
            Position = user.Position,
            RegisteredLocationId = user.RegisteredLocationId,
            PositionOrderWeight = user.PositionOrderWeight!.Value,
            Location = user.Location,
            ResourceGroupId = user.ResourceGroupId
        };

        _teamBoardContract
            .Setup(x => x.ArePersonalPhoneAndEmailsUniqueAsync(requestModel.PersonalEmail,
                requestModel.PersonalEmailSecond, requestModel.PersonalPhone, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _canRoleHaveDepartmentBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _canUserWithRoleBeCreatedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanCurrentUserCreateUserWithRoleBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _canUserWithResourceGroupBeCreatedBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanUserWithResourceGroupBeCreatedBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _teamBoardContract.Setup(x => x.GetCountries(_cancellationToken))
            .ReturnsAsync([new CountryOutput { Id = 1, Name = "Belarus" }]);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>().BuildMockDbSet().Object);

        _identityDbContextMock
            .Setup(x => x.Users.Add(It.IsAny<User>()))
            .Returns(entityEntry.Object);

        _rolesCacheMock
            .Setup(x => x.GetAllRoles(_cancellationToken))
            .ReturnsAsync([
                new() { Id = 1, Name = user.Position, }
            ]);

        _canRoleBeOccupiedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanRoleBeOccupiedBusinessRule.Request>(),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _userLoaderMock
            .Setup(x => x.LoadAsync(user.Id, _cancellationToken))
            .ReturnsAsync(userDto);

        // Act
        var result = await _commandHandler.Handle(
            requestModel,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(userDto);

        _outboxContractMock.Verify(
            x => x.PublishAsync(
                It.Is<UserCreatedEvent>(
                    y => y.UserId == userCreatedNotification.UserId
                         && y.FirstName == userCreatedNotification.FirstName
                         && y.LastName == userCreatedNotification.LastName
                         && y.Email == userCreatedNotification.Email
                         && y.DepartmentId == userCreatedNotification.DepartmentId),
                _cancellationToken),
            Times.Once);
    }
}
