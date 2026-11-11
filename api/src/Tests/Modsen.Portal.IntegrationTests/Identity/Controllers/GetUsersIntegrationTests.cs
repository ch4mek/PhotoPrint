using System.Net;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Identity.Controllers;

[Collection("GeneralCollection")]
public class GetUsersIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetUsersIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetCurrentUserShouldReturnExpectedModel()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var expected = new UserOutput
        {
            Email = EmailConstants.EmployeeEmail,
            FirstName = "Test",
            LastName = "Employee",
            FullName = "Employee Test",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            RightPosition = PortalRoleNames.Employee,
            TimeZone = "BY",
            DepartmentId = "Net",
            IsActive = true,
            CountryId = PortalSharedConstants.DefaultCountryId,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var actual = await client.GetCurrentUser();

        // Assert
        actual.Should().BeEquivalentTo(expected, config =>
        {
            // Exclude random values
            config.Excluding(x => x.Color);
            config.Excluding(x => x.Id);
            config.Excluding(x => x.FirstName);

            return config;
        });
    }

    [Fact]
    public async Task GetHeadOfDepartmentUserShouldReturnExpectedModel()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var expected = new UserOutput
        {
            Email = EmailConstants.HeadEmail,
            FirstName = "Test",
            LastName = "HeadOfDepartment",
            FullName = "HeadOfDepartment Test",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.HeadOfDepartment,
            RightPosition = "Head of Department",
            TimeZone = "BY",
            DepartmentId = "Net",
            IsActive = true,
            CountryId = PortalSharedConstants.DefaultCountryId,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        await client.AuthAsHeadOfDepartment();

        // Act
        var actual = await client.GetCurrentUser();

        // Assert
        actual.Should().BeEquivalentTo(expected, config =>
        {
            // Exclude random values
            config.Excluding(x => x.Color);
            config.Excluding(x => x.Id);
            config.Excluding(x => x.FirstName);

            return config;
        });
    }

    [Fact]
    public async Task GetUsersLookupWithRoleFilterIncludingCeoCtoHeadReturnListOfHeadsCtoCeo()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var expectedDescriptions = new[]{ "CEO", "CTO", "Head of Department" };
        await client.AuthAsHeadOfDepartment();

        // Act
        var actual = await client.GetUsersLookup(roles: expectedDescriptions, isActive: true);

        // Assert
        actual.Should().Contain(x=> expectedDescriptions.Any(y => y == x.PositionDescription));
    }

    [Fact]
    public async Task GetUsersLookupWithoutFiltersReturnAllUsers()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var expected = new List<UserLookupOutput>
        {
            new()
            {
                FirstName= "Test",
                LastName = "CEO"
            },
            new()
            {
                FirstName = "Test",
                LastName = "CTO"
            },
            new()
             {
                FirstName = "Test",
                LastName  = "Employee",
                DepartmentId = "Net"
             },
             new()
             {
                FirstName= "Test",
                LastName = "HeadOfDepartment",
                DepartmentId = "Net"
             },
             new()
             {
                FirstName = "Test",
                LastName = "SysAdmin"
             }
        };

        await client.AuthAsCto();

        // Act
        var actual = await client.GetUsersLookup();

        // Assert
        actual.Should().NotBeNull();
        expected.ForEach(expectedUser => actual.Should().ContainEquivalentOf(expectedUser, config =>
        {
            config.Excluding(x => x.AvatarFileId);
            config.Excluding(x => x.Color);
            config.Excluding(x => x.Id);
            config.Excluding(x => x.PositionDescription);
            config.Excluding(x => x.DepartmentId);
            config.Excluding(x => x.IsActive);

            return config;
        }));
    }

    [Fact]
    public async Task GetUsersLookupWhenAuthorizedAsEmployeeReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsEmployee();

        // Act
        var response = await client.GetUsersLookupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUsersLookupWithKeyWordSearchPatternReturnAppropriateUser()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var expected = new List<UserLookupOutput>
        {
            new()
            {
                DepartmentId = "Net",
                FirstName = "Test",
                LastName = "HeadOfDepartment"
            }
        };

        await client.AuthAsCto();

        // Act
        var actual = await client.GetUsersLookup(keyWord: "HeadOfDepartment");

        // Assert
        actual.Should().BeEquivalentTo(expected, config =>
        {
            config.Excluding(x => x.AvatarFileId);
            config.Excluding(x => x.Color);
            config.Excluding(x => x.Id);
            config.Excluding(x => x.PositionDescription);
            config.Excluding(x => x.DepartmentId);
            config.Excluding(x => x.IsActive);

            return config;
        });
    }

    [Fact]
    public async Task GetUsersLookupWhenAuthAsHeadReturnUsersFromHeadsDepartment()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var expected = new List<UserLookupOutput>
        {
            new()
            {
                LastName = "HeadOfDepartment",
                FirstName= "Test",
                DepartmentId = "Net"
            },
            new ()
            {
                LastName = "Employee",
                FirstName = "Test",
                DepartmentId = "Net"
            }
        };

        var unexpectedUsers = new List<UserLookupOutput>
        {
            new()
            {
                LastName = "CEO",
                FirstName = "Test"
            },
            new()
            {
                LastName = "CTO",
                FirstName = "Test"
            },
            new()
            {
                FirstName = "Marina",
                LastName = "Krivel",
                DepartmentId = "Js"

            },
            new()
            {
                FirstName = "Test",
                LastName = "SysAdmin"
            }
        };

        await client.AuthAsHeadOfDepartment();

        // Act
        var actual = await client.GetUsersLookup();

        // Assert
        expected.ForEach(expectedUser => actual.Should().ContainEquivalentOf(expectedUser, config =>
        {
            config.Excluding(x => x.AvatarFileId);
            config.Excluding(x => x.Color);
            config.Excluding(x => x.Id);
            config.Excluding(x => x.PositionDescription);
            config.Excluding(x => x.DepartmentId);
            config.Excluding(x => x.IsActive);
            return config;
        }));
        unexpectedUsers.ForEach(unexpectedUser => actual.Should().NotContainEquivalentOf(unexpectedUser, config =>
        {
            config.Excluding(x => x.AvatarFileId);
            config.Excluding(x => x.Color);
            config.Excluding(x => x.Id);
            config.Excluding(x => x.PositionDescription);
            config.Excluding(x => x.DepartmentId);
            config.Excluding(x => x.IsActive);
            return config;
        }));
    }

    [Fact]
    public async Task GetUsersLookupWhenAuthAsGroupManagerReturnUsersFromHisGroup()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createdUserFromGroup = await client.CreateEmployee(departmentId: "Net", resourceGroupId: 1);
        var createdUserNotFromGroup = await client.CreateEmployee(departmentId: "Java", resourceGroupId: 2);

        var unexpectedUsersIds = new[]
        {
            _factory.CeoUserId, _factory.CtoUserId, _factory.EmployeeUserId, _factory.HeadOfDepartmentUserId,
            _factory.SystemUserId, createdUserNotFromGroup!.Id
        };
        var expectedUsersIds = new[]
        {
            _factory.GroupManagerId, createdUserFromGroup.Id
        };

        await client.AuthAsGroupManager();

        // Act
        var actual = await client.GetUsersLookup();

        // Assert
        actual.Should().NotBeNull();
        expectedUsersIds.All(id => actual.Any(x => x.Id == id)).Should().BeTrue();
        unexpectedUsersIds.All(id => actual.Any(x => x.Id == id)).Should().BeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetUsersLookupWhenOnlyIsActiveFilterSpecifiedShouldReturnRelevantUsers(bool isActive)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var actualUsers = await client.GetUsersLookup(isActive: isActive);

        // Assert
        actualUsers.Should().NotBeNull();
        actualUsers.Should().Match(x => !x.Any() || x.All(u => u.IsActive == isActive));
    }

    [Fact]
    public async Task GetUserByIdWhenAuthorizedAsEmployeeReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsEmployee();

        // Act
        var response = await client.GetUserAsHttpMessage(1);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserByIdAsSystemAdministratorWhenUserExistsReturnUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsAdmin();
        var userId = _factory.SystemUserId!.Value;

        var expectedUser = new UserOutput
        {
            Email = "support@modsen-software.com",
            FirstName = "System",
            LastName = "User",
            FullName = "User System",
            Id = _factory.SystemUserId!.Value,
            Location = LocationType.Vitebsk,
            Position = PortalRoleNames.Employee,
            RightPosition = PortalRoleNames.Employee,
            IsActive = true,
            CountryId = PortalSharedConstants.DefaultCountryId,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var response = await client.GetUserAsync(userId);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expectedUser, config =>
        {
            config.Excluding(x => x.Color);
            config.Excluding(x => x.AvatarFileId);
            config.Excluding(x => x.TimeZone);
            config.Excluding(x => x.DepartmentId);

            return config;
        });
    }

    [Fact]
    public async Task GetUserByIdWhenUserExistReturnUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();
        var userId = _factory.SystemUserId!.Value;

        var expectedUser = new UserOutput
        {
            Email = "support@modsen-software.com",
            FirstName = "System",
            LastName = "User",
            FullName = "User System",
            Id = _factory.SystemUserId!.Value,
            Location = LocationType.Vitebsk,
            Position = PortalRoleNames.Employee,
            RightPosition = PortalRoleNames.Employee,
            IsActive = true,
            CountryId = PortalSharedConstants.DefaultCountryId,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var response = await client.GetUserAsync(userId);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expectedUser, config =>
        {
            config.Excluding(x => x.Color);
            config.Excluding(x => x.AvatarFileId);
            config.Excluding(x => x.TimeZone);
            config.Excluding(x => x.DepartmentId);

            return config;
        });
    }

    [Fact]
    public async Task GetUserByIdWhenAuthorizedAsHeadFromOtherDepartmentReturnNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetUserAsHttpMessage(1);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserByIdWhenAuthorizedAsGmFromOtherGroupReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createdUserNotFromGroup = await client.CreateEmployee(
            departmentId: "Java",
            resourceGroupId: 2);

        await client.AuthAsGroupManager();

        // Act
        var response = await client.GetUserAsHttpMessage(createdUserNotFromGroup!.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?")]
    [InlineData("?top=50")]
    [InlineData("?skip=0")]
    [InlineData("?top=0&skip=50")]
    [InlineData("?top=50&skip=-1")]
    [InlineData("?top=50&skip=15")]
    [InlineData("?top=26&skip=50")]
    [InlineData("?top=50&skip=100&select=id,firstName")]
    [InlineData("?top=50&skip=0&apply=groupby((position), aggregate(id with count as Count))")]
    public async Task GetUsersBySearchPatternWhenRequestIsInvalidResponseWithBadRequestResult(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetUsersWithRawSearchAsHttpMessage(query: query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetUsersBySearchPatternWhenFilterContainsUserPropertyWithTesValueShouldReturnOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string filter = "contains(FullName, tolower('TES'))";

        // Act
        var response = await client.GetUsersWithSearchAsHttpMessage(filter: filter);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUsersBySearchPatternWhenRequestIsValidResponseWithExpectedUsersCountResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string searchPattern = "PattenFirstNameOne";
        const string filter = $"contains(firstName,'{searchPattern}')";
        const string orderBy = "id";

        await client.CreateEmployee(firstName: searchPattern);

        // Act
        var users = await client.GetUsersWithSearch(
            filter: filter,
            orderBy: orderBy);

        // Assert
        users.Should().NotBeNull();
        users.Page.Should().Be(1);
        users.Items.Count.Should().BeGreaterThanOrEqualTo(1);
        users.Items.Should()
            .OnlyContain(user =>
                user.FirstName != null &&
                user.FirstName == searchPattern);
    }

    [Fact]
    public async Task GetUsersBySearchPatternWhenRequestedBySystemAdministratorResponseWithExpectedUsersCountResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string searchPattern = "PattenFirstNameTwo";
        const string filter = $"contains(firstName,'{searchPattern}')";
        const string orderBy = "id";

        await client.CreateEmployee(firstName: searchPattern);

        await client.AuthAsAdmin();

        // Act
        var users = await client.GetUsersWithSearch(
            filter: filter,
            orderBy: orderBy);

        // Assert
        users.Should().NotBeNull();
        users.Page.Should().Be(1);
        users.Items.Count.Should().BeGreaterThanOrEqualTo(1);
        users.Items.Should()
            .OnlyContain(user =>
                user.FirstName != null &&
                user.FirstName == searchPattern);
    }

    [Fact]
    public async Task GetUsersBySearchPatternWithPostMethodWhenRequestIsValidResponseWithExpectedUsersCountResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string searchPattern = "PattenFirstNameThree";
        const string query = $"$orderBy=id&$filter=contains(firstName,'{searchPattern}')";

        await client.CreateEmployee(firstName: searchPattern);

        // Act
        var users = await client.GetUsersWithSearchAsPostMethod(query: query);

        // Assert
        users.Should().NotBeNull();
        users.Page.Should().Be(1);
        users.Items.Count.Should().BeGreaterThanOrEqualTo(1);
        users.Items.Should()
            .OnlyContain(user =>
                user.FirstName == searchPattern);
    }

    [Fact]
    public async Task GetUsersBySearchPatternWhenHeadTryToGetUsersReturnUsersFromHisDepartment()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var searchPattern = "PattenFirstNameFour";
        var filter = $"contains(firstName,'{searchPattern}')";

        await client.CreateEmployee(firstName: searchPattern);
        await client.CreateEmployee(firstName: searchPattern, departmentId: "Js");

        await client.AuthAsHeadOfDepartment();

        // Act
        var users = await client.GetUsersWithSearch(filter: filter);

        // Assert
        users.Should().NotBeNull();
        users.Page.Should().Be(1);
        users.Items.Count.Should().Be(1);
        users.Items.Should()
            .OnlyContain(user =>
                user.FirstName == searchPattern &&
                user.DepartmentId == "Net");
    }

    [Fact]
    public async Task GetUsersBySearchPatternWhenGroupManagerTryToGetUsersReturnUsersFromHisGroup()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var searchPattern = $"TestSearchGroupManagerPattern{Guid.CreateVersion7().ToAlphabetic()}";
        var filter = $"contains(firstName,'{searchPattern}')";

        await client.CreateEmployee(firstName: searchPattern, departmentId: "Net",
            resourceGroupId: 1);

        await client.CreateEmployee(firstName: searchPattern, departmentId: "Java",
            resourceGroupId: 2);

        await client.AuthAsGroupManager();

        // Act
        var users = await client.GetUsersWithSearch(filter: filter);

        // Assert
        users.Should().NotBeNull();
        users.Page.Should().Be(1);
        users.Items.Count.Should().Be(1);
        users.Items.Should()
            .OnlyContain(user =>
                user.FirstName == searchPattern &&
                user.ResourceGroupId == 1);
    }

    [Fact]
    public async Task GetUsersBySearchPatternWhenRequestIsValidResponseWithExpectedUsersTotalCount()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var firstName = $"FirstName{Guid.CreateVersion7().ToAlphabetic()}";
        var filter = $"contains(firstName,'{firstName}')";

        await client.CreateEmployee(firstName: $"{firstName}{Guid.CreateVersion7().ToAlphabetic()}", departmentId: "Js");
        await client.CreateEmployee(firstName: $"{firstName}{Guid.CreateVersion7().ToAlphabetic()}", departmentId: "Net");

        // Act
        var users = await client.GetUsersWithSearch(filter: filter,
            top: 1);

        // Assert
        users.Should().NotBeNull();
        users.Page.Should().Be(1);
        users.Items.Count.Should().Be(1);
        users.Total.Should().Be(2);
    }

    [Theory]
    [MemberData(nameof(GetOrderedUsersTestData))]
    public async Task GetUsersBySearchPatternWithDifferentOrdersResponseWithExpectedUsersCountResult(List<CreateUserInput> usersToAdd, List<UserOutput> expectedUsers, string orderBy, bool orderByDesc)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        foreach (var user in usersToAdd)
        {
            var createdUser = await client.CreateUser(user);

            expectedUsers.First(x => x.Email == user.Email).Id = createdUser!.Id;
        }

        var filter = "((isActive eq true) and " + string.Join(" or ", usersToAdd.Select(x => $"contains(email,'{x.Email}')")) + ")";

        // Act
        var users = await client.GetUsersWithSearch(filter: filter,
                orderBy: orderBy + " " + (orderByDesc ? "desc" : "asc"));

        // Assert
        foreach (var user in users.Items)
        {
            await client.DeleteUserAsync(user.Id);
        }

        users.Page.Should().Be(1);
        users.Items.Count.Should().Be(3);
        users.Items.Should()
            .BeEquivalentTo(expectedUsers,
                config =>
                {
                    config.Excluding(x => x.FirstNameRu);
                    config.Excluding(x => x.LastNameRu);
                    config.Excluding(x => x.FullNameRu);
                    config.Excluding(x => x.Color);
                    config.Excluding(x => x.AvatarFileId);
                    config.Excluding(x => x.Email);
                    config.Excluding(x => x.Position);
                    config.Excluding(x => x.TimeZone);
                    config.Excluding(x => x.DepartmentId);
                    config.Excluding(x => x.Location);
                    config.Excluding(x => x.Position);
                    config.Excluding(x => x.RightPosition);
                    config.Excluding(x => x.IsActive);
                    config.Excluding(x => x.RegisteredLocationId);

                    return config;
                });
    }

    public static IEnumerable<object?[]> GetOrderedUsersTestData()
    {
        yield return GenerateTestData("fullName", true);
        yield return GenerateTestData("fullName", false);
        yield return GenerateTestData("email", true);
        yield return GenerateTestData("email", false);
        yield return GenerateTestData("id", true);
        yield return GenerateTestData("id", false);
        yield return GenerateTestData("location", true);
        yield return GenerateTestData("location", false);
        yield return GenerateTestData("PositionOrderWeight", true);
        yield return GenerateTestData("PositionOrderWeight", false);
    }

    private static object[] GenerateTestData(string orderBy, bool orderByDesc)
    {
        var usersForCreating = new List<CreateUserInput>
        {
            new ()
            {
                FirstName = "Test",
                LastName = "AAA",
                FirstNameRu = "Имя",
                LastNameRu = "Фамилия",
                PatronymicRu = "Отчество",
                Color = "#f47835",
                Email = $"AAA{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                Position = PortalRoleNames.SystemAdministrator,
                Location = LocationType.Minsk,
                DepartmentId = "Administration",
                CountryId = 1,
                RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            },
            new ()
            {
                FirstName = "Flo",
                LastName = "OOO",
                FirstNameRu = "Имя",
                LastNameRu = "Фамилия",
                PatronymicRu = "Отчество",
                Color = "#be29ec",
                Email = $"OOO{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                Position = PortalRoleNames.Employee,
                Location = LocationType.Tbilisi,
                DepartmentId = "Administration",
                CountryId = 1,
                RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            },
            new ()
            {
                FirstName = "Cleo",
                LastName = "ZZZ",
                FirstNameRu = "Имя",
                LastNameRu = "Фамилия",
                PatronymicRu = "Отчество",
                Color = "#f7c531",
                Email = $"ZZZ{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                Position = PortalRoleNames.Employee,
                Location = LocationType.Vitebsk,
                DepartmentId = "Python",
                CountryId = 1,
                RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            }
        };

        var usersOutputAscending = usersForCreating.Select(x => new UserOutput
        {
            Id = 1,
            Email = x.Email!,
            FirstName = x.FirstName,
            LastName = x.LastName,
            FullName = $"{x.LastName} {x.FirstName}"
        }).ToList();

        var usersOutputDescending = new List<UserOutput>
        {
            usersOutputAscending[2],
            usersOutputAscending[1],
            usersOutputAscending[0]
        };

        var expectedUsers = orderByDesc ? usersOutputDescending : usersOutputAscending;

        return [usersForCreating, expectedUsers, orderBy, orderByDesc];
    }
}
