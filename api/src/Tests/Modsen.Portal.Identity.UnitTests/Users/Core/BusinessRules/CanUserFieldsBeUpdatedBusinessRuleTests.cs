using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Core.BusinessRules;

public class CanUserFieldsBeUpdatedBusinessRuleTests
{
    private readonly ICanUserFieldsBeUpdatedBusinessRule _businessRule;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CanUserFieldsBeUpdatedBusinessRuleTests()
    {
        _businessRule = new CanUserFieldsBeUpdatedBusinessRule(_authenticatedUserContextMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee, PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Ceo, PortalRoleNames.Employee)]
    public async Task IsCorrespondsToBusinessWhenCeoPermissionsAreInvalidReturnBusinessRuleFailResult(
        string requestPosition, string userPosition)
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: requestPosition,
            Location: LocationType.Minsk,
            DepartmentId: null,
            ResourceGroupId: null,
            CurrentUserRole: [PortalRoleNames.Ceo],
            CurrentUserDepartmentId: null,
            HireDate: null);

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [userPosition],
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCannotChangeRoleToCeo);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee, PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.Employee, PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.Ceo, PortalRoleNames.Ceo)]
    public async Task IsCorrespondsToBusinessWhenCeoPermissionsAreValidReturnBusinessRuleSuccessResult(
        string requestPosition, string userPosition)
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: requestPosition,
            Location: LocationType.Minsk,
            DepartmentId: null,
            ResourceGroupId: null,
            CurrentUserRole: [PortalRoleNames.Ceo],
            CurrentUserDepartmentId: null,
            HireDate: null);

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [userPosition],
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Reason.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee, PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Ceo, PortalRoleNames.Employee)]
    public async Task IsCorrespondsToBusinessWhenCtoPermissionsAreInvalidReturnBusinessRuleFailResult(
        string requestPosition, string userPosition)
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: requestPosition,
            Location: LocationType.Minsk,
            DepartmentId: null,
            ResourceGroupId: null,
            CurrentUserRole: [PortalRoleNames.Cto],
            CurrentUserDepartmentId: null,
            HireDate: null);

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [userPosition],
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCannotEditUsersWithCeoRole);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee, PortalRoleNames.GroupManager)]
    [InlineData(PortalRoleNames.GroupManager, PortalRoleNames.Employee)]
    public async Task IsCorrespondsToBusinessWhenCtoPermissionsAreValidReturnBusinessRuleSuccessResult(
        string requestPosition, string userPosition)
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: requestPosition,
            Location: LocationType.Minsk,
            DepartmentId: null,
            ResourceGroupId: null,
            CurrentUserRole: [PortalRoleNames.Cto],
            CurrentUserDepartmentId: null,
            HireDate: null);

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [userPosition],
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Reason.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto, PortalRoleNames.Cto)]

    public async Task IsCorrespondsToBusinessWhenCurrentUserIsHeadAndUserForUpdateIsCeoOrCtoReturnBusinessRuleFailResult(
        string requestPosition, string userPosition)
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: requestPosition,
            Location: LocationType.Minsk,
            DepartmentId: "Net",
            ResourceGroupId: null,
            CurrentUserRole: [PortalRoleNames.HeadOfDepartment],
            CurrentUserDepartmentId: "Net",
            HireDate: null);

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [userPosition],
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCanEditEmployeesOnlyFromYourDepartment);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserIsHeadAndUserForUpdateIsFromAnotherDepartmentReturnBusinessRuleFailResult()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: PortalRoleNames.Employee,
            Location: LocationType.Minsk,
            DepartmentId: "Js",
            ResourceGroupId: null,
            CurrentUserRole: [PortalRoleNames.HeadOfDepartment],
            CurrentUserDepartmentId: "Net",
            HireDate: null
            );

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Js",
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCanEditEmployeesOnlyFromYourDepartment);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserIsHeadAndDepartmentIsChangedReturnBusinessRuleFailResult()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: PortalRoleNames.Employee,
            Location: LocationType.Minsk,
            DepartmentId: "Js",
            ResourceGroupId: null,
            CurrentUserRole: [PortalRoleNames.HeadOfDepartment],
            CurrentUserDepartmentId: "Net",
            HireDate: null);

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net",
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCanEditEmployeesOnlyFromYourDepartment);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserIsHeadAndRoleIsChangedReturnBusinessRuleFailResult()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: PortalRoleNames.GroupManager,
            Location: LocationType.Minsk,
            DepartmentId: "Net",
            ResourceGroupId: null,
            CurrentUserRole: [PortalRoleNames.HeadOfDepartment],
            CurrentUserDepartmentId: "Net",
            HireDate: null);

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net",
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCanEditEmployeesOnlyFromYourDepartment);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserIsHeadAndPermissionsAreValidReturnBusinessRuleSuccessResult()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: PortalRoleNames.Employee,
            Location: LocationType.Minsk,
            DepartmentId: "Net",
            ResourceGroupId: null,
            CurrentUserRole: [PortalRoleNames.HeadOfDepartment],
            CurrentUserDepartmentId: "Net",
            HireDate: null
            );

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net",
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Reason.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserIsSysAdminAndRoleIsChangedReturnBusinessRuleFailResult()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: "FirstName",
            LastName: "LastName",
            FirstNameRu: "Имя",
            LastNameRu: "Фамилия",
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: "example@email.com",
            Position: PortalRoleNames.GroupManager,
            Location: LocationType.Minsk,
            DepartmentId: "Net",
            ResourceGroupId: null,
            CurrentUserRole: [PortalRoleNames.SystemAdministrator],
            CurrentUserDepartmentId: "Net",
            HireDate: null
            );

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net",
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCannotChangeUsersRole);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserIsSysAdminAndDepartmentIsChangedReturnBusinessRuleFailResult()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: "FirstName",
            LastName: "LastName",
            FirstNameRu: "Имя",
            LastNameRu: "Фамилия",
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: "example@email.com",
            Position: PortalRoleNames.Employee,
            Location: LocationType.Minsk,
            DepartmentId: "Js",
            ResourceGroupId: null,
            CurrentUserRole: [PortalRoleNames.SystemAdministrator],
            CurrentUserDepartmentId: "Net",
            HireDate: null
            );

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net",
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCannotChangeUsersDepartment);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserIsSysAdminAndGroupIsChangedReturnBusinessRuleFailResult()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: "FirstName",
            LastName: "LastName",
            FirstNameRu: "Имя",
            LastNameRu: "Фамилия",
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: "example@email.com",
            Position: PortalRoleNames.Employee,
            Location: LocationType.Minsk,
            DepartmentId: "Net",
            ResourceGroupId: 1,
            CurrentUserRole: [PortalRoleNames.SystemAdministrator],
            CurrentUserDepartmentId: "Net",
            HireDate: null
            );

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net",
            ResourceGroupId = 2
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCannotChangeUsersResourceGroup);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserIsSysAdminAndLocationIsChangedReturnBusinessRuleFailResult()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: "FirstName",
            LastName: "LastName",
            FirstNameRu: "Имя",
            LastNameRu: "Фамилия",
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: "example@email.com",
            Position: PortalRoleNames.Employee,
            Location: LocationType.Minsk,
            DepartmentId: "Net",
            ResourceGroupId: null,
            CurrentUserRole: [PortalRoleNames.SystemAdministrator],
            CurrentUserDepartmentId: "Net",
            HireDate: null
            );

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net",
            Location = LocationType.Vitebsk,
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCannotChangeUsersLocation);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserIsSysAdminAndPermissionsAreValidReturnBusinessRuleSuccessResult()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: "FirstName",
            LastName: "LastName",
            FirstNameRu: "Имя",
            LastNameRu: "Фамилия",
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: "example@email.com",
            Position: PortalRoleNames.Employee,
            Location: LocationType.Minsk,
            DepartmentId: "Net",
            ResourceGroupId: 1,
            CurrentUserRole: [PortalRoleNames.SystemAdministrator],
            CurrentUserDepartmentId: "Net",
            HireDate: null
            );

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net",
            ResourceGroupId = 1,
            Location = LocationType.Minsk,
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Reason.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto, PortalRoleNames.Cto)]

    public async Task IsCorrespondsToBusinessWhenCurrentUserIsGroupManagerAndUserForUpdateIsCeoOrCtoReturnBusinessRuleFailResult(
        string requestPosition, string userPosition)
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: requestPosition,
            Location: LocationType.Minsk,
            DepartmentId: "Net",
            ResourceGroupId: 1,
            CurrentUserRole: [PortalRoleNames.GroupManager],
            CurrentUserDepartmentId: "Net",
            HireDate: null
            );

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [userPosition],
            ResourceGroupId = 1,
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCanEditEmployeesOnlyFromYourGroup);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserIsGroupManagerAndUserForUpdateIsFromAnotherDepartmentReturnBusinessRuleFailResult()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: PortalRoleNames.Employee,
            Location: LocationType.Minsk,
            DepartmentId: "Js",
            ResourceGroupId: 1,
            CurrentUserRole: [PortalRoleNames.GroupManager],
            CurrentUserDepartmentId: "Net",
            HireDate: null
            );

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Js",
            ResourceGroupId = 1,
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCanEditEmployeesOnlyFromYourGroup);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserIsGroupManagerAndDepartmentIsChangedReturnBusinessRuleFailResult()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: PortalRoleNames.Employee,
            Location: LocationType.Minsk,
            DepartmentId: "Js",
            ResourceGroupId: 1,
            CurrentUserRole: [PortalRoleNames.GroupManager],
            CurrentUserDepartmentId: "Net",
            HireDate: null
            );

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net",
            ResourceGroupId = 1,
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCanEditEmployeesOnlyFromYourGroup);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserIsGroupManagerAndGroupIsChangedReturnBusinessRuleFailResult()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: PortalRoleNames.Employee,
            Location: LocationType.Minsk,
            DepartmentId: "Net",
            ResourceGroupId: 1,
            CurrentUserRole: [PortalRoleNames.GroupManager],
            CurrentUserDepartmentId: "Net",
            HireDate: null
            );

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net",
            ResourceGroupId = 2,
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo(PortalMessages.YouCannotChangeUsersResourceGroup);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCurrentUserIsGroupManagerAndPermissionsAreValidReturnBusinessRuleSuccessResult()
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            Id: 10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: null,
            CountryId: null,
            RegisteredLocationId: null,
            CurrentLocation: null,
            Email: null,
            Position: PortalRoleNames.Employee,
            Location: LocationType.Minsk,
            DepartmentId: "Net",
            ResourceGroupId: 1,
            CurrentUserRole: [PortalRoleNames.HeadOfDepartment],
            CurrentUserDepartmentId: "Net",
            HireDate: null
            );

        var userDto = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net",
            ResourceGroupId = 1,
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            new CanUserFieldsBeUpdatedBusinessRule.Request(userDto, requestModel),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Reason.Should().BeNullOrEmpty();
    }
}
