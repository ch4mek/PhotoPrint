using System.Net;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.Identity.Controllers.UserController
{
    [Collection("GeneralCollection")]
    public class GetUserTabLookupIntegrationTests
    {
        private readonly IntegrationTestFactory _factory;

        public GetUserTabLookupIntegrationTests(IntegrationTestFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetUserTabLookupWhenAuthAsGroupManagerReturnUsersFromHisGroup()
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
                createdUserFromGroup.Id
            };

            await client.AuthAsGroupManager();

            // Act
            var actual = await client.GetUserTabLookup();

            // Assert
            actual.Should().NotBeNull();
            expectedUsersIds.All(id => actual.Any(x => x.Id == id)).Should().BeTrue();
            unexpectedUsersIds.All(id => actual.Any(x => x.Id == id)).Should().BeFalse();
        }

        [Fact]
        public async Task GetUserTabLookupWithoutFiltersReturnAllUsers()
        {
            // Arrange
            using var client = _factory.CreateClient();
            var expected = new List<UserLookupWithRussianNamesShortOutput>
            {
                new()
                 {
                    FirstName = "Test",
                    LastName  = "Employee",
                    Id = _factory.EmployeeUserId!.Value,
                 },
                 new()
                 {
                    FirstName= "Test",
                    LastName = "HeadOfDepartment",
                    Id = _factory.HeadOfDepartmentUserId!.Value,
                 },
                 new()
                 {
                    FirstName = "Test",
                    LastName = "SysAdmin",
                    Id = _factory.SystemUserId!.Value,
                 }
            };

            await client.AuthAsCto();

            // Act
            var actual = await client.GetUserTabLookup();

            // Assert
            actual.Should().NotBeNull();
            expected.ForEach(expectedUser => actual.Should().ContainEquivalentOf(expectedUser, config =>
            {
                config.Excluding(x => x.FirstName);
                config.Excluding(x => x.LastName);
                config.Excluding(x => x.Id);

                return config;
            }));
        }

        [Fact]
        public async Task GetUserTabLookupWhenAuthorizedAsEmployeeReturnForbidden()
        {
            // Arrange
            using var client = _factory.CreateClient();

            await client.AuthAsEmployee();

            // Act
            var response = await client.GetUserTabLookupAsHttpMessage();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetUserTabLookupWithKeyWordSearchPatternReturnAppropriateUser()
        {
            // Arrange
            using var client = _factory.CreateClient();

            var expected = new List<UserLookupWithRussianNamesShortOutput>
            {
                new()
                {
                    Id = _factory.HeadOfDepartmentUserId!.Value,
                    FirstName = "Test",
                    LastName = "HeadOfDepartment"
                }
            };

            await client.AuthAsCto();

            // Act
            var actual = await client.GetUserTabLookup(keyWord: "HeadOfDepartment");

            // Assert
            actual.Should().BeEquivalentTo(expected, config =>
            {
                config.Excluding(x => x.FirstName);
                config.Excluding(x => x.LastName);
                config.Excluding(x => x.Id);

                return config;
            });
        }

        [Fact]
        public async Task GetUserTabLookupWhenAuthAsHeadReturnUsersFromHeadsDepartment()
        {
            // Arrange
            using var client = _factory.CreateClient();

            var expected = new List<UserLookupWithRussianNamesShortOutput>
            {
                new ()
                {
                    Id = _factory.EmployeeUserId!.Value,
                    LastName = "Employee",
                    FirstName = "Test",
                }
            };

            var unexpectedUsers = new List<UserLookupWithRussianNamesShortOutput>
            {
                new()
                {
                    LastName = "CEO",
                    FirstName = "Test",
                    Id = _factory.CeoUserId!.Value,
                },
                new()
                {
                    LastName = "CTO",
                    FirstName = "Test",
                    Id = _factory.CtoUserId!.Value,
                },
                new()
                {
                    Id = _factory.HeadOfDepartmentUserId!.Value,
                    LastName = "HeadOfDepartment",
                    FirstName= "Test",
                }
            };

            await client.AuthAsHeadOfDepartment();

            // Act
            var actual = await client.GetUserTabLookup();

            // Assert
            expected.ForEach(expectedUser => actual.Should().ContainEquivalentOf(expectedUser, config =>
            {
                config.Excluding(x => x.FirstName);
                config.Excluding(x => x.LastName);
                config.Excluding(x => x.Id);
                return config;
            }));
            unexpectedUsers.ForEach(unexpectedUser => actual.Should().NotContainEquivalentOf(unexpectedUser, config =>
            {
                config.Excluding(x => x.Id);
                return config;
            }));
        }
    }
}
