using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Home.Core.Converters;
using Modsen.Portal.TeamBoard.Requests.Home.Requests;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Home;

public class GetAllBirthdaysRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();

    private readonly GetAllBirthdaysRequest _handler;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetAllBirthdaysRequestTests()
    {
        _handler = new GetAllBirthdaysRequest(
            _dbContextMock.Object,
            new TeamboardUserToDepartmentNameConverter(),
            _clockServiceMock.Object);
    }

    [Fact]
    public async Task HandleWhenYearIsLeapReturnsValidBirthdays()
    {
        // Arrange
        _clockServiceMock
            .Setup(service => service.UtcNow)
            .Returns(new DateTime(2004, 1, 1));

        var ceoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Ceo,
            FirstName = "CEO first name",
            LastName = "CEO last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth = new DateOnly(1996, 2, 29),
            Department = null
        };

        var ctoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Cto,
            FirstName = "CTO first name",
            LastName = "CTO last name",
            Email = "test",
            DateOfBirth = new DateOnly(1993, 2, 28),
            Department = null
        };

        var headUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.HeadOfDepartment,
            FirstName = "Head first name",
            LastName = "Head last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth = new DateOnly(2000, 4, 11),
            Department = new TeamboardDepartment { FullName = ".NET", DepartmentId = "Net", ShortName = ".NET" }
        };

        var groupManagerUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.GroupManager,
            FirstName = "GroupManager first name",
            LastName = "GroupManager last name",
            Email = "test",
            DateOfBirth =  new DateOnly(1991, 6, 2),
            Department = new TeamboardDepartment { FullName = ".NET", DepartmentId = "Net", ShortName = ".NET" }
        };

        var sysAdminUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.SystemAdministrator,
            FirstName = "SysAdmin first name",
            LastName = "SysAdmin last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth =  new DateOnly(1993, 6, 2),
            Department = new TeamboardDepartment { FullName = "System Administrators", DepartmentId = "Net", ShortName = ".NET" }
        };

        var employeeUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Employee,
            FirstName = "Employee first name",
            LastName = "Employee last name",
            Email = "test",
            DateOfBirth =  new DateOnly(1993, 12, 4),
            Department = new TeamboardDepartment { FullName = "DevOps", DepartmentId = "Net", ShortName = ".NET" }
        };

        _dbContextMock
            .Setup(context => context.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                ceoUser, ctoUser, headUser, groupManagerUser, sysAdminUser, employeeUser
            }.BuildMockDbSet().Object);

        var expectedResult = new List<MonthWithBirthdaysDto>
        {
            new()
            {
                Month = 2,
                Days =
                [
                    new DayWithBirthdaysDto
                    {
                        Day = 28,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = ctoUser.LastName,
                                FirstName = ctoUser.FirstName,
                                AvatarFileId = ctoUser.AvatarFileId,
                                Department = PortalRoleNames.Cto,
                            }
                        ]
                    },
                    new DayWithBirthdaysDto
                    {
                        Day = 29,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = ceoUser.LastName,
                                FirstName = ceoUser.FirstName,
                                AvatarFileId = ceoUser.AvatarFileId,
                                Department = PortalRoleNames.Ceo,
                            }
                        ]
                    }
                ]
            },
            new()
            {
                Month = 4,
                Days = [
                    new DayWithBirthdaysDto
                    {
                        Day = 11,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = headUser.LastName,
                                FirstName = headUser.FirstName,
                                AvatarFileId = headUser.AvatarFileId,
                                Department = $"Head of {headUser.Department.FullName}",
                            }
                        ]
                    }
                ]
            },
            new()
            {
                Month = 6,
                Days = [
                    new DayWithBirthdaysDto
                    {
                        Day = 2,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = groupManagerUser.LastName,
                                FirstName = groupManagerUser.FirstName,
                                AvatarFileId = groupManagerUser.AvatarFileId,
                                Department = groupManagerUser.Department.FullName,
                            },
                            new UserBirthdayDto
                            {
                                LastName = sysAdminUser.LastName,
                                FirstName = sysAdminUser.FirstName,
                                AvatarFileId = sysAdminUser.AvatarFileId,
                                Department = sysAdminUser.Department.FullName,
                            },
                        ]
                    }
                ]
            },
            new()
            {
                Month = 12,
                Days = [
                    new DayWithBirthdaysDto
                    {
                        Day = 4,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = employeeUser.LastName,
                                FirstName = employeeUser.FirstName,
                                AvatarFileId = employeeUser.AvatarFileId,
                                Department = employeeUser.Department.FullName,
                            }
                        ]
                    }
                ]
            }
        };

        //Act
        var birthdays = await _handler.Handle(
            new GetAllBirthdaysRequest.Query(null, null), _token);

        //Assert
        birthdays.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenYearIsNotLeapAndThereIs28ThFebruaryBirthdaysReturnsValidBirthdays()
    {
        // Arrange
        _clockServiceMock
            .Setup(service => service.UtcNow)
            .Returns(new DateTime(2005, 1, 1));

        var ceoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Ceo,
            FirstName = "CEO first name",
            LastName = "CEO last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth = new DateOnly(1996, 2, 29),
            Department = null
        };

        var ctoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Cto,
            FirstName = "CTO first name",
            LastName = "CTO last name",
            Email = "test",
            DateOfBirth = new DateOnly(1993, 2, 28),
            Department = null
        };

        var headUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.HeadOfDepartment,
            FirstName = "Head first name",
            LastName = "Head last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth = new DateOnly(2000, 4, 11),
            Department = new TeamboardDepartment { FullName = ".NET", DepartmentId = "Net", ShortName = ".NET" }
        };

        var groupManagerUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.GroupManager,
            FirstName = "GroupManager first name",
            LastName = "GroupManager last name",
            Email = "test",
            DateOfBirth =  new DateOnly(1991, 6, 2),
            Department = new TeamboardDepartment { FullName = ".NET", DepartmentId = "Net", ShortName = ".NET" }
        };

        var sysAdminUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.SystemAdministrator,
            FirstName = "SysAdmin first name",
            LastName = "SysAdmin last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth =  new DateOnly(1993, 6, 2),
            Department = new TeamboardDepartment { FullName = "System Administrators", DepartmentId = "Net", ShortName = ".NET" }
        };

        var employeeUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Employee,
            FirstName = "Employee first name",
            LastName = "Employee last name",
            Email = "test",
            DateOfBirth =  new DateOnly(1993, 12, 4),
            Department = new TeamboardDepartment { FullName = "DevOps", DepartmentId = "Net", ShortName = ".NET" }
        };

        _dbContextMock
            .Setup(context => context.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                ceoUser, ctoUser, headUser, groupManagerUser, sysAdminUser, employeeUser
            }.BuildMockDbSet().Object);

        var expectedResult = new List<MonthWithBirthdaysDto>
        {
            new()
            {
                Month = 2,
                Days =
                [
                    new DayWithBirthdaysDto
                    {
                        Day = 28,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = ctoUser.LastName,
                                FirstName = ctoUser.FirstName,
                                AvatarFileId = ctoUser.AvatarFileId,
                                Department = PortalRoleNames.Cto,
                            },
                            new UserBirthdayDto
                            {
                                LastName = ceoUser.LastName,
                                FirstName = ceoUser.FirstName,
                                AvatarFileId = ceoUser.AvatarFileId,
                                Department = PortalRoleNames.Ceo,
                            }
                        ]
                    },
                ]
            },
            new()
            {
                Month = 4,
                Days = [
                    new DayWithBirthdaysDto
                    {
                        Day = 11,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = headUser.LastName,
                                FirstName = headUser.FirstName,
                                AvatarFileId = headUser.AvatarFileId,
                                Department = $"Head of {headUser.Department.FullName}",
                            }
                        ]
                    }
                ]
            },
            new()
            {
                Month = 6,
                Days = [
                    new DayWithBirthdaysDto
                    {
                        Day = 2,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = groupManagerUser.LastName,
                                FirstName = groupManagerUser.FirstName,
                                AvatarFileId = groupManagerUser.AvatarFileId,
                                Department = groupManagerUser.Department.FullName,
                            },
                            new UserBirthdayDto
                            {
                                LastName = sysAdminUser.LastName,
                                FirstName = sysAdminUser.FirstName,
                                AvatarFileId = sysAdminUser.AvatarFileId,
                                Department = sysAdminUser.Department.FullName,
                            },
                        ]
                    }
                ]
            },
            new()
            {
                Month = 12,
                Days = [
                    new DayWithBirthdaysDto
                    {
                        Day = 4,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = employeeUser.LastName,
                                FirstName = employeeUser.FirstName,
                                AvatarFileId = employeeUser.AvatarFileId,
                                Department = employeeUser.Department.FullName,
                            }
                        ]
                    }
                ]
            }
        };

        //Act
        var birthdays = await _handler.Handle(
            new GetAllBirthdaysRequest.Query(null, null), _token);

        //Assert
        birthdays.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenYearIsNotLeapAndThereIsNo28ThFebruaryBirthdaysReturnsValidBirthdays()
    {
        // Arrange
        _clockServiceMock
            .Setup(service => service.UtcNow)
            .Returns(new DateTime(2005, 1, 1));

        var ceoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Ceo,
            FirstName = "CEO first name",
            LastName = "CEO last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth = new DateOnly(1996, 2, 29),
            Department = null
        };

        var ctoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Cto,
            FirstName = "CTO first name",
            LastName = "CTO last name",
            Email = "test",
            DateOfBirth = new DateOnly(1993, 2, 26),
            Department = null
        };

        var headUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.HeadOfDepartment,
            FirstName = "Head first name",
            LastName = "Head last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth = new DateOnly(2000, 4, 11),
            Department = new TeamboardDepartment { FullName = ".NET", DepartmentId = "Net", ShortName = ".NET" }
        };

        var groupManagerUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.GroupManager,
            FirstName = "GroupManager first name",
            LastName = "GroupManager last name",
            Email = "test",
            DateOfBirth =  new DateOnly(1991, 6, 2),
            Department = new TeamboardDepartment { FullName = ".NET", DepartmentId = "Net", ShortName = ".NET" }
        };

        var sysAdminUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.SystemAdministrator,
            FirstName = "SysAdmin first name",
            LastName = "SysAdmin last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth =  new DateOnly(1993, 6, 2),
            Department = new TeamboardDepartment { FullName = "System Administrators", DepartmentId = "Net", ShortName = ".NET" }
        };

        var employeeUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Employee,
            FirstName = "Employee first name",
            LastName = "Employee last name",
            Email = "test",
            DateOfBirth =  new DateOnly(1993, 12, 4),
            Department = new TeamboardDepartment { FullName = "DevOps", DepartmentId = "Net", ShortName = ".NET" }
        };

        _dbContextMock
            .Setup(context => context.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                ceoUser, ctoUser, headUser, groupManagerUser, sysAdminUser, employeeUser
            }.BuildMockDbSet().Object);

        var expectedResult = new List<MonthWithBirthdaysDto>
        {
            new()
            {
                Month = 2,
                Days =
                [
                    new DayWithBirthdaysDto
                    {
                        Day = 26,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = ctoUser.LastName,
                                FirstName = ctoUser.FirstName,
                                AvatarFileId = ctoUser.AvatarFileId,
                                Department = PortalRoleNames.Cto,
                            }
                        ]
                    },
                    new DayWithBirthdaysDto
                    {
                        Day = 28,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = ceoUser.LastName,
                                FirstName = ceoUser.FirstName,
                                AvatarFileId = ceoUser.AvatarFileId,
                                Department = PortalRoleNames.Ceo,
                            }
                        ]
                    },
                ]
            },
            new()
            {
                Month = 4,
                Days = [
                    new DayWithBirthdaysDto
                    {
                        Day = 11,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = headUser.LastName,
                                FirstName = headUser.FirstName,
                                AvatarFileId = headUser.AvatarFileId,
                                Department = $"Head of {headUser.Department.FullName}",
                            }
                        ]
                    }
                ]
            },
            new()
            {
                Month = 6,
                Days = [
                    new DayWithBirthdaysDto
                    {
                        Day = 2,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = groupManagerUser.LastName,
                                FirstName = groupManagerUser.FirstName,
                                AvatarFileId = groupManagerUser.AvatarFileId,
                                Department = groupManagerUser.Department.FullName,
                            },
                            new UserBirthdayDto
                            {
                                LastName = sysAdminUser.LastName,
                                FirstName = sysAdminUser.FirstName,
                                AvatarFileId = sysAdminUser.AvatarFileId,
                                Department = sysAdminUser.Department.FullName,
                            },
                        ]
                    }
                ]
            },
            new()
            {
                Month = 12,
                Days = [
                    new DayWithBirthdaysDto
                    {
                        Day = 4,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = employeeUser.LastName,
                                FirstName = employeeUser.FirstName,
                                AvatarFileId = employeeUser.AvatarFileId,
                                Department = employeeUser.Department.FullName,
                            }
                        ]
                    }
                ]
            }
        };

        //Act
        var birthdays = await _handler.Handle(
            new GetAllBirthdaysRequest.Query(null, null), _token);

        //Assert
        birthdays.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenMonthProvidedReturnsValidBirthdays()
    {
        // Arrange
        _clockServiceMock
            .Setup(service => service.UtcNow)
            .Returns(new DateTime(2004, 1, 1));

        var ceoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Ceo,
            FirstName = "CEO first name",
            LastName = "CEO last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth = new DateOnly(1996, 2, 29),
            Department = null
        };

        var ctoUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Cto,
            FirstName = "CTO first name",
            LastName = "CTO last name",
            Email = "test",
            DateOfBirth = new DateOnly(1993, 2, 28),
            Department = null
        };

        var headUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.HeadOfDepartment,
            FirstName = "Head first name",
            LastName = "Head last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth = new DateOnly(2000, 4, 11),
            Department = new TeamboardDepartment { FullName = ".NET", DepartmentId = "Net", ShortName = ".NET" }
        };

        var groupManagerUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.GroupManager,
            FirstName = "GroupManager first name",
            LastName = "GroupManager last name",
            Email = "test",
            DateOfBirth =  new DateOnly(1991, 6, 2),
            Department = new TeamboardDepartment { FullName = ".NET", DepartmentId = "Net", ShortName = ".NET" }
        };

        var sysAdminUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.SystemAdministrator,
            FirstName = "SysAdmin first name",
            LastName = "SysAdmin last name",
            Email = "test",
            AvatarFileId = "filed",
            DateOfBirth =  new DateOnly(1993, 6, 2),
            Department = new TeamboardDepartment { FullName = "System Administrators", DepartmentId = "Net", ShortName = ".NET" }
        };

        var employeeUser = new TeamBoardUser
        {
            IsActive = true,
            Position = PortalRoleNames.Employee,
            FirstName = "Employee first name",
            LastName = "Employee last name",
            Email = "test",
            DateOfBirth =  new DateOnly(1993, 12, 4),
            Department = new TeamboardDepartment { FullName = "DevOps", DepartmentId = "Net", ShortName = ".NET" }
        };

        _dbContextMock
            .Setup(context => context.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                ceoUser, ctoUser, headUser, groupManagerUser, sysAdminUser, employeeUser
            }.BuildMockDbSet().Object);

        var expectedResult = new List<MonthWithBirthdaysDto>
        {
            new()
            {
                Month = 6,
                Days = [
                    new DayWithBirthdaysDto
                    {
                        Day = 2,
                        Users =
                        [
                            new UserBirthdayDto
                            {
                                LastName = groupManagerUser.LastName,
                                FirstName = groupManagerUser.FirstName,
                                AvatarFileId = groupManagerUser.AvatarFileId,
                                Department = groupManagerUser.Department.FullName,
                            },
                            new UserBirthdayDto
                            {
                                LastName = sysAdminUser.LastName,
                                FirstName = sysAdminUser.FirstName,
                                AvatarFileId = sysAdminUser.AvatarFileId,
                                Department = sysAdminUser.Department.FullName,
                            },
                        ]
                    }
                ]
            },
        };

        //Act
        var birthdays = await _handler.Handle(
            new GetAllBirthdaysRequest.Query(null, 6), _token);

        //Assert
        birthdays.Should().BeEquivalentTo(expectedResult);
    }
}
