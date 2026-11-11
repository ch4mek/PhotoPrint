using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Departments.Core;
using Modsen.Portal.Identity.Requests.Departments.Core.CoreDto;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Departments;

public class DepartmentLoaderTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IDepartmentsCache> _departmentsCacheMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly IDepartmentLoader _departmentsLoader;

    public DepartmentLoaderTests()
    {
        _departmentsLoader = new DepartmentLoader(_identityDbContextMock.Object, _departmentsCacheMock.Object);
    }

    [Fact]
    public async Task LoadDepartmentsClassificationsReturnValidDepartmentsClassificationsList()
    {
        // Arrange

        var netGroup = new DepartmentGroupCoreDto
        {
            Id = 1,
            Name = "test group",
            DepartmentId = "Net",
            GroupManagerEmail = "net.gm@test.com"
        };

        var netGroups = new List<ResourceGroup>
        {
            new()
            {
                IsActive = true,
                DepartmentId = netGroup.DepartmentId,
                Name = netGroup.Name,
                Id = netGroup.Id.Value
            }
        };

        var emptyGroupList = new List<DepartmentGroupCoreDto>();

        var expectedNetGroups = new List<DepartmentGroupCoreDto>
        {
            new()
            {
                GroupManagerEmail = netGroup.GroupManagerEmail,
                Id = netGroup.Id,
                Name = netGroup.Name,
                DepartmentId = netGroup.DepartmentId
            }
        };

        var headAndGmUsers = new List<User>
        {
            new()
            {
                FirstName = "firstName",
                LastName = "test",
                Email = "js.head@test.com",
                DepartmentId = "Js",
                IsActive = true,
                UserRoles = new List<UserRole>()
                {
                    new()
                    {
                        Role = new Role
                        {
                            Name = PortalRoleNames.HeadOfDepartment,
                            PublicName = "Test"
                        }
                    }
                }
            },
            new()
            {
                FirstName = "firstName",
                LastName = "test",
                Email = "net.head@test.com",
                DepartmentId = "Net",
                IsActive = true,
                UserRoles = new List<UserRole>()
                {
                    new()
                    {
                        Role = new Role
                        {
                            Name = PortalRoleNames.HeadOfDepartment,
                            PublicName = "Test"
                        }
                    }
                }
            },
            new()
            {
                FirstName = "firstName",
                LastName = "test",
                Email = "qa.head@test.com",
                DepartmentId = "QA",
                IsActive = true,
                UserRoles = new List<UserRole>()
                {
                    new()
                    {
                        Role = new Role
                        {
                            Name = PortalRoleNames.HeadOfDepartment,
                            PublicName = "Test"
                        }
                    }
                }
            },
            new()
            {
                FirstName = "firstName",
                LastName = "test",
                Email = netGroup.GroupManagerEmail,
                DepartmentId = netGroup.DepartmentId,
                ResourceGroupId = netGroup.Id,
                IsActive = true,
                UserRoles = new List<UserRole>()
                {
                    new()
                    {
                        Role = new Role
                        {
                            Name = PortalRoleNames.GroupManager,
                            PublicName = "Test"
                        }
                    }
                }
            }
        };

        var departments = new List<DepartmentCacheRecord>
        {
            GetDepartmentDataDto("Js","Js","JavaScript","Js"),
            GetDepartmentDataDto("Net","Net",".NET","Net"),
            GetDepartmentDataDto("Java","Java","Java","Java"),
            GetDepartmentDataDto("Python","Python","Python","Python"),
            GetDepartmentDataDto("QA","QA","QA","QA"),
            GetDepartmentDataDto("BA","BA","BA","BA"),
            GetDepartmentDataDto("HR","HR","HR","HR"),
            GetDepartmentDataDto("Marketing","Marketing","Marketing","Marketing"),
            GetDepartmentDataDto("Sales","Sales","Sales","Sales"),
            GetDepartmentDataDto("PMO","PMO","PMO","PMO"),
            GetDepartmentDataDto("Mobile","Mobile","Mobile","Mobile"),
            GetDepartmentDataDto("PHP","PHP","PHP","PHP"),
            GetDepartmentDataDto("DevOps","DevOps","DevOps","DevOps"),
            GetDepartmentDataDto("Design","Design","Design","Design"),
            GetDepartmentDataDto("Recruitment", "Recruitment", "Recruitment", "Recruitment")
        };

        var expectedResult = new List<DepartmentCoreDto>
        {
            GetDepartmentCoreDto("JavaScript", emptyGroupList, "js.head@test.com"),
            GetDepartmentCoreDto(".NET", expectedNetGroups,"net.head@test.com"),
            GetDepartmentCoreDto("Java", emptyGroupList),
            GetDepartmentCoreDto("Python", emptyGroupList),
            GetDepartmentCoreDto("QA", emptyGroupList,"qa.head@test.com"),
            GetDepartmentCoreDto("BA", emptyGroupList),
            GetDepartmentCoreDto("HR", emptyGroupList),
            GetDepartmentCoreDto("Marketing", emptyGroupList),
            GetDepartmentCoreDto("Sales", emptyGroupList),
            GetDepartmentCoreDto("PMO", emptyGroupList),
            GetDepartmentCoreDto("Mobile", emptyGroupList),
            GetDepartmentCoreDto("PHP", emptyGroupList),
            GetDepartmentCoreDto("DevOps", emptyGroupList),
            GetDepartmentCoreDto("Design", emptyGroupList),
            GetDepartmentCoreDto("Recruitment", emptyGroupList)
        };

        var usersDbSetMock = headAndGmUsers.BuildMockDbSet();

        var groupsDbSetMock = netGroups.BuildMockDbSet();

        _identityDbContextMock.Setup(x => x.Users).Returns(usersDbSetMock.Object);

        _identityDbContextMock.Setup(x => x.ResourceGroups).Returns(groupsDbSetMock.Object);

        _departmentsCacheMock
            .Setup(x => x.GetAllDepartments(_cancellationToken))
            .ReturnsAsync(departments);

        // Act
        var result = await _departmentsLoader
            .LoadDepartments(_cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult, config => config.WithStrictOrdering());

        static DepartmentCacheRecord GetDepartmentDataDto(string id, string shortName,
            string fullName, string modsenApiValue) => new()
        {
                Id = id,
                FullName = fullName,
                ModsenApiValue = modsenApiValue,
                ShortName = shortName,
                OrderWeight = 1
            };

        static DepartmentCoreDto GetDepartmentCoreDto(
            string departmentName,
            List<DepartmentGroupCoreDto>? groups,
            string? headOfDepartmentId = null) => new()
        {
                DepartmentName = departmentName,
                Groups = groups,
                HeadOfDepartmentId = headOfDepartmentId
            };
    }

    [Fact]
    public async Task LoadDepartmentsWhenSeveralHeadsExistReturnValidDepartmentsList()
    {
        // Arrange

        var departments = new List<DepartmentCacheRecord>
        {
            GetDepartmentDataDto("Js","Js","JavaScript","Js"),
            GetDepartmentDataDto("Net","Net",".NET","Net"),
            GetDepartmentDataDto("Java","Java","Java","Java"),
            GetDepartmentDataDto("Python","Python","Python","Python"),
            GetDepartmentDataDto("QA","QA","QA","QA"),
            GetDepartmentDataDto("BA","BA","BA","BA"),
            GetDepartmentDataDto("HR","HR","HR","HR"),
            GetDepartmentDataDto("Marketing","Marketing","Marketing","Marketing"),
            GetDepartmentDataDto("Sales","Sales","Sales","Sales"),
            GetDepartmentDataDto("PMO","PMO","PMO","PMO"),
            GetDepartmentDataDto("Mobile","Mobile","Mobile","Mobile"),
            GetDepartmentDataDto("PHP","PHP","PHP","PHP"),
            GetDepartmentDataDto("DevOps","DevOps","DevOps","DevOps"),
            GetDepartmentDataDto("Design","Design","Design","Design")
        };

        var expectedResult = departments.Select(x => new DepartmentClassificationCoreDto
        (x.Id, x.ShortName, x.FullName)).ToList();

        _departmentsCacheMock
            .Setup(x => x.GetAllDepartments(_cancellationToken))
            .ReturnsAsync(departments);

        // Act
        var result = await _departmentsLoader
            .LoadDepartmentsClassifications(_cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult, config => config.WithStrictOrdering());

        static DepartmentCacheRecord GetDepartmentDataDto(string id, string shortName,
            string fullName, string modsenApiValue) => new()
        {
                Id = id,
                FullName = fullName,
                ModsenApiValue = modsenApiValue,
                ShortName = shortName,
                OrderWeight = 1
            };
    }
}
