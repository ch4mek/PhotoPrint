using System.Globalization;
using Microsoft.Extensions.Configuration;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.Contract;
using Modsen.Portal.Inquiries.Contract.Models;
using Modsen.Portal.TeamBoard.Requests.Payload.Core;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.Loaders;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Contract.ContractDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Payload.Core;

public class EmployeesPayloadComposerTests
{
    private readonly Mock<IPayloadUserDetailsLoader> _payloadUserDetailsLoaderMock = new();
    private readonly Mock<IPayloadCalendarWorkingHourDetailsLoader> _payloadCalendarWorkingHourDetailsLoaderMock = new();
    private readonly Mock<ITimeTrackContract> _timeTrackContractMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly Mock<IClockService> _clockService = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IInquiriesContract> _inquiriesContractMock = new();
    private readonly WorkingDaysCalculator _workingDaysCalculator = new();
    private readonly UserProjectHoursCalculator _userProjectHoursCalculator;
    private readonly IConfiguration _configuration;
    private readonly IsUserAssignedBusinessRule _isUserAssignedBusinessRule = new();
    private readonly WorkingHoursInMonthCalculator _workingHoursInMonthCalculator = new();
    private readonly UserHoursCalculator _userHoursCalculator = new(new WorkingDaysCalculator());
    private readonly EmployeesPayloadComposer _composer;
    private readonly TotalUserTimeByMonthsCalculator _totalUserTimeByMonthsCalculator;
    private readonly UserActivityMonthsCalculator _userActivityMonthsCalculator = new();
    private readonly int _defaultCountryId = 1;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly int _minutesInHour = 60;

    private readonly CalendarDto _calendarDetails = new()
    {
        Countries =
        [
            new() { Id = 1, Name = "Belarus", }
        ],
        CalendarWorkingHourDetails =
        [
            new() {
                MonthOfYear = 1,
                CountryHours =
                [
                    new() {
                        CountryId = 1,
                        Hours = 156,
                    }
                ],
            },
            new() {
                MonthOfYear = 2,
                CountryHours =
                [
                    new() {
                        CountryId = 1,
                        Hours = 182,
                    }
                ],
            },
            new() {
                MonthOfYear = 3,
                CountryHours =
                [
                    new() {
                        CountryId = 1,
                        Hours = 160,
                    }
                ],
            },
            new() {
                MonthOfYear = 4,
                CountryHours =
                [
                    new() {
                        CountryId = 1,
                        Hours = 172,
                    }
                ],
            },
            new() {
                MonthOfYear = 5,
                CountryHours =
                [
                    new() {
                        CountryId = 1,
                        Hours = 200,
                    }
                ],
            },
            new() {
                MonthOfYear = 6,
                CountryHours =
                [
                    new() {
                        CountryId = 1,
                        Hours = 170,
                    }
                ],
            },
            new() {
                MonthOfYear = 7,
                CountryHours =
                [
                    new() {
                        CountryId = 1,
                        Hours = 184,
                    }
                ],
            },
            new() {
                MonthOfYear = 8,
                CountryHours =
                [
                    new() {
                        CountryId = 1,
                        Hours = 144,
                    }
                ],
            },
            new() {
                MonthOfYear = 9,
                CountryHours =
                [
                    new() {
                        CountryId = 1,
                        Hours = 176,
                    }
                ],
            },
            new() {
                MonthOfYear = 10,
                CountryHours =
                [
                    new() {
                        CountryId = 1,
                        Hours = 172,
                    }
                ],
            },
            new() {
                MonthOfYear = 11,
                CountryHours =
                [
                    new() {
                        CountryId = 1,
                        Hours = 156,
                    }
                ],
            },
            new() {
                MonthOfYear = 12,
                CountryHours =
                [
                    new() {
                        CountryId = 1,
                        Hours = 168,
                    }
                ],
            },
        ],
    };

    public EmployeesPayloadComposerTests()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"AppSettings:TeamboardSettings:DefaultCountryId", _defaultCountryId.ToString(CultureInfo.InvariantCulture) },
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _payloadCalendarWorkingHourDetailsLoaderMock
            .Setup(mock => mock.Load(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(_calendarDetails);

        _userProjectHoursCalculator = new(_workingDaysCalculator);
        _totalUserTimeByMonthsCalculator = new(_workingDaysCalculator);

        _composer = new EmployeesPayloadComposer(
            _payloadUserDetailsLoaderMock.Object,
            _payloadCalendarWorkingHourDetailsLoaderMock.Object,
            _timeTrackContractMock.Object,
            _configuration,
            _workingDaysCalculator,
            _userProjectHoursCalculator,
            _isUserAssignedBusinessRule,
            _workingHoursInMonthCalculator,
            _userHoursCalculator,
            _totalUserTimeByMonthsCalculator,
            _userActivityMonthsCalculator,
            _clockService.Object,
            _identityContractMock.Object,
            _authenticatedUserContextMock.Object,
            _inquiriesContractMock.Object);
    }

    [Fact]
    public async Task HandleWhenOneAssignInYearShouldReturnCorrectValue()
    {
        //Arrange
        var userIds = new[] { 1 };
        var year = 2025;
        var filter = new PayloadFilterCoreDto();

        var userId = 1;
        var isUserNeedProject = false;
        var userComment = "Comment";
        var userFirstName = "FirstName";
        var userLastName = "LastName";
        var userColor = "#ffffff";
        var userPrimarySpecialization = "userPrimarySpecialization";
        var userDepartment = ".Net";

        var projectId = 1;
        var projectName = "ProjectName";
        var projectFrom = new DateOnly(2025, 2, 10);
        var projectTo = new DateOnly(2025, 4, 5);
        var isActiveProject = true;

        var clientId = 1;
        var clientName = "ClientName";
        var clientColor = "#ffffff";

        var users = new UserDetailsCoreDto[]
        {
            new()
            {
                UserId = userId,
                Comment = userComment,
                FirstName = userFirstName,
                LastName = userLastName,
                Color = userColor,
                PrimarySpecializationName = userPrimarySpecialization,
                DepartmentId = userDepartment,
                RegisteredLocations =
                [
                    new RegisteredLocationHistoryCoreDto
                    {
                        DateFrom = projectFrom.AddMonths(-1),
                        RegisteredLocationId = 1
                    }
                ],
                ProjectDetails =
                [
                    new() {
                        ProjectId = projectId,
                        Fte = 0.5m,
                        UserOnProjectFrom = projectFrom,
                        UserOnProjectTo = projectTo,
                        IsTimeOnProjectBillable = true,
                    }
                ],
                UserMonthPayloadDetails =
                [
                    new() {
                        EstimatedFte = 0.5m,
                        PayloadMonthOfYear = 2,
                    },
                    new() {
                        EstimatedFte = 0.5m,
                        PayloadMonthOfYear = 3,
                    },
                ]
            },
        };

        var payload = new PayloadContractDto
        {
            Year = year,
            Users =
            [
                new() {
                    UserId = userId,
                    HourDetails =
                    [
                        new() {
                            MonthOfYear = 2,
                            TotalBillableTrackedInMinutes = 10 * _minutesInHour,
                            TotalFactTrackedInMinutes = 10 * _minutesInHour,
                            Projects =
                            [
                                new() {
                                    IsActive = isActiveProject,
                                    UserOnProjectFrom = projectFrom,
                                    UserOnProjectTo = projectTo,
                                    ProjectId = projectId,
                                    TotalBillableTrackedInMinutes = 10 * _minutesInHour,
                                    TotalFactTrackedInMinutes = 10 * _minutesInHour,
                                }
                            ],
                        },
                        new() {
                            MonthOfYear = 3,
                            TotalBillableTrackedInMinutes = 4 * _minutesInHour,
                            TotalFactTrackedInMinutes = 4 * _minutesInHour,
                            Projects =
                            [
                                new() {
                                    IsActive = true,
                                    UserOnProjectFrom = projectFrom,
                                    UserOnProjectTo = projectTo,
                                    ProjectId = projectId,
                                    TotalBillableTrackedInMinutes = 4 * _minutesInHour,
                                    TotalFactTrackedInMinutes = 4 * _minutesInHour,
                                }
                            ],
                        }
                    ],
                }
            ],
            Projects =
                [
                    new() {
                        ProjectId = projectId,
                        ClientId = clientId,
                        Name = projectName,
                    }
                ],
            Clients =
                [
                    new() {
                        ClientId = clientId,
                        Name = clientName,
                        Color = clientColor,
                    }
                ],
        };

        var ownExpenseAbsence = new List<UserInquiryDto>
        {
            new()
            {
                Id = 1,
                UserId = userId,
                InquiryTemplateName = "OwnExpense",
                InquiryType = InquiryType.OwnExpense,
                DateFrom = new DateOnly(2025, 2, 9),
                DateTo = new DateOnly(2025, 2, 12),
                Status = UserInquiryStatus.Approved
            }
        };

        var calendarDetails = new List<CalendarExclusionDto>
        {
            new()
            {
                Id = 1,
                Name = "DayOff",
                Date = new DateOnly(2025, 2, 7),
                NewDate = new DateOnly(2025, 2, 9),
                DateType = DateType.DayOff,
                CountryId = 1
            },
            new()
            {
                Id = 1,
                Name = "Holiday",
                Date = new DateOnly(2025, 2, 10),
                NewDate = null,
                DateType = DateType.Holiday,
                CountryId = 1
            },
        };

        _payloadUserDetailsLoaderMock
            .Setup(mock => mock.Load(It.IsAny<int[]>(), year, null, _cancellationToken))
            .ReturnsAsync(users);

        _timeTrackContractMock
            .Setup(mock => mock.GetTrackedTimePayloadForUsers(It.IsAny<int[]>(), year, null, null, _cancellationToken))
            .ReturnsAsync(payload);

        _inquiriesContractMock
            .Setup(mock => mock.GetUsersInquiriesAsync(It.IsAny<int[]>(), _cancellationToken, It.IsAny<InquiryType>(), It.IsAny<UserInquiryStatus>()))
            .ReturnsAsync(ownExpenseAbsence);

        _inquiriesContractMock
            .Setup(mock => mock.GetCalendarExclusionAsync(It.IsAny<int>(), _cancellationToken, null))
            .ReturnsAsync(calendarDetails);

        _clockService
            .Setup(mock => mock.TodayUtc())
            .Returns(new DateOnly(2025, 3, 10));

        //Act
        var result = await _composer.Compose(year, userIds, userIds.Length, filter, _cancellationToken);

        //Assert
        result.Year.Should().Be(year);

        foreach (var project in result.Projects)
        {
            project.ProjectId.Should().Be(projectId);
            project.Name.Should().Be(projectName);
            project.ClientId.Should().Be(clientId);
        }

        foreach (var client in result.Clients)
        {
            client.ClientId.Should().Be(clientId);
            client.Name.Should().Be(clientName);
            client.Color.Should().Be(clientColor);
        }

        var user = result.Users.First();

        user.NeedProject.Should().Be(isUserNeedProject);
        user.Comments.Should().Be(userComment);
        user.FirstName.Should().Be(userFirstName);
        user.LastName.Should().Be(userLastName);
        user.Color.Should().Be(userColor);
        user.DepartmentId.Should().Be(userDepartment);
        user.SpecializationName.Should().Be(userPrimarySpecialization);

        foreach (var project in user.Projects)
        {
            project.IsActiveAssignement.Should().Be(isActiveProject);
            project.ProjectId.Should().Be(projectId);
        }

        user.HourDetails.Count.Should().Be(12);

        foreach (var hourDetail in user.HourDetails)
        {
            if (hourDetail.MonthOfYear == 2)
            {
                hourDetail.ActualFte.Should().Be(0.06m);
                hourDetail.PlannedFte.Should().Be(0.5m);
                hourDetail.PlannedWorkloadInHours.Should().Be((_calendarDetails.CalendarWorkingHourDetails[1].CountryHours[0].Hours - 24) * 0.5m);
                hourDetail.ProjectDetails.Count.Should().Be(1);
                hourDetail.RequiredHours.Should().Be(59.25m);
                hourDetail.TrackedBillableInHours.Should().Be(10);

                var projectDetail = hourDetail.ProjectDetails.First();

                projectDetail.ProjectId.Should().Be(projectId);
                projectDetail.ActualFte.Should().Be(0.06m);
                projectDetail.TrackedBillableInHours.Should().Be(10);
                projectDetail.RequiredHours.Should().Be(59.25m);
            }
            else if (hourDetail.MonthOfYear == 3)
            {
                hourDetail.ActualFte.Should().Be(0.02m);
                hourDetail.PlannedFte.Should().Be(0.5m);
                hourDetail.PlannedWorkloadInHours.Should().Be(_calendarDetails.CalendarWorkingHourDetails[2].CountryHours[0].Hours * 0.5m);
                hourDetail.ProjectDetails.Count.Should().Be(1);
                hourDetail.RequiredHours.Should().Be(80m);
                hourDetail.TrackedBillableInHours.Should().Be(4);

                var projectDetail = hourDetail.ProjectDetails.First();

                projectDetail.ProjectId.Should().Be(projectId);
                projectDetail.ActualFte.Should().Be(0.02m);
                projectDetail.TrackedBillableInHours.Should().Be(4);
                projectDetail.RequiredHours.Should().Be(80m);
            }
            else if (hourDetail.MonthOfYear == 4)
            {
                hourDetail.ActualFte.Should().Be(0m);
                hourDetail.PlannedFte.Should().Be(0m);
                hourDetail.PlannedWorkloadInHours.Should().Be(0m);
                hourDetail.ProjectDetails.Count.Should().Be(1);
                hourDetail.RequiredHours.Should().Be(15.64m);
                hourDetail.TrackedBillableInHours.Should().Be(0);

                var projectDetail = hourDetail.ProjectDetails.First();

                projectDetail.ProjectId.Should().Be(projectId);
                projectDetail.ActualFte.Should().Be(0m);
                projectDetail.TrackedBillableInHours.Should().Be(0);
                projectDetail.RequiredHours.Should().Be(15.64m);
            }
            else
            {
                hourDetail.ActualFte.Should().Be(0);
                hourDetail.PlannedFte.Should().Be(0);
                hourDetail.PlannedWorkloadInHours.Should().Be(0);
                hourDetail.ProjectDetails.Count.Should().Be(0);
                hourDetail.RequiredHours.Should().Be(0);
                hourDetail.TrackedBillableInHours.Should().Be(0);
            }
        }
    }

    [Fact]
    public async Task HandleWhenTwoAssignInYearShouldReturnCorrectValue()
    {
        //Arrange
        var userIds = new[] { 1 };
        var year = 2025;
        var filter = new PayloadFilterCoreDto();

        var userId = 1;
        var isUserNeedProject = false;
        var userComment = "Comment";
        var userFirstName = "FirstName";
        var userLastName = "LastName";
        var userColor = "#ffffff";
        var userPrimarySpecialization = "userPrimarySpecialization";
        var userDepartment = ".Net";

        var projectId = 1;
        var projectName = "ProjectName";
        var projectFrom = new DateOnly(2025, 2, 10);
        var projectTo = new DateOnly(2025, 4, 5);
        var isActiveProject = true;
        var isFutureActive = false;

        var project2Id = 2;
        var projectName2 = "ProjectName";
        var projectFrom2 = new DateOnly(2025, 7, 5);
        var projectTo2 = new DateOnly(2025, 7, 10);
        var isActiveProject2 = false;
        var isFutureActive2 = true;

        var clientId = 1;
        var clientName = "ClientName";
        var clientColor = "#ffffff";

        var users = new UserDetailsCoreDto[]
        {
            new()
            {
                UserId = userId,
                Comment = userComment,
                FirstName = userFirstName,
                LastName = userLastName,
                Color = userColor,
                PrimarySpecializationName = userPrimarySpecialization,
                DepartmentId = userDepartment,
                RegisteredLocations =
                [
                    new RegisteredLocationHistoryCoreDto
                    {
                        DateFrom = projectFrom.AddMonths(-1),
                        RegisteredLocationId = 1
                    }
                ],
                ProjectDetails =
                [
                    new() {
                        ProjectId = projectId,
                        Fte = 0.5m,
                        UserOnProjectFrom = projectFrom,
                        UserOnProjectTo = projectTo,
                        IsTimeOnProjectBillable = true,
                    },
                    new() {
                        ProjectId = project2Id,
                        Fte = 0.75m,
                        UserOnProjectFrom = projectFrom2,
                        UserOnProjectTo = projectTo2,
                        IsTimeOnProjectBillable = true,
                    }
                ],
                UserMonthPayloadDetails =
                [
                    new() {
                        EstimatedFte = 0.5m,
                        PayloadMonthOfYear = 2,
                    },
                    new() {
                        EstimatedFte = 0.5m,
                        PayloadMonthOfYear = 3,
                    },
                ],
            },
        };

        var payload = new PayloadContractDto
        {
            Year = year,
            Users =
            [
                new() {
                    UserId = userId,
                    HourDetails =
                    [
                        new() {
                            MonthOfYear = 2,
                            TotalBillableTrackedInMinutes = 10 * _minutesInHour,
                            TotalFactTrackedInMinutes = 10 * _minutesInHour,
                            Projects =
                            [
                                new() {
                                    IsActive = isActiveProject,
                                    UserOnProjectFrom = projectFrom,
                                    UserOnProjectTo = projectTo,
                                    ProjectId = projectId,
                                    TotalBillableTrackedInMinutes = 10 * _minutesInHour,
                                    TotalFactTrackedInMinutes = 10 * _minutesInHour,
                                }
                            ],
                        },
                        new() {
                            MonthOfYear = 3,
                            TotalBillableTrackedInMinutes = 4 * _minutesInHour,
                            TotalFactTrackedInMinutes = 4 * _minutesInHour,
                            Projects =
                            [
                                new() {
                                    IsActive = true,
                                    UserOnProjectFrom = projectFrom,
                                    UserOnProjectTo = projectTo,
                                    ProjectId = projectId,
                                    TotalBillableTrackedInMinutes = 4 * _minutesInHour,
                                    TotalFactTrackedInMinutes = 4 * _minutesInHour,
                                }
                            ],
                        },
                        new() {
                            MonthOfYear = 7,
                            TotalBillableTrackedInMinutes = 0,
                            TotalFactTrackedInMinutes = 0,
                            Projects =
                            [
                                new() {
                                    IsActive = true,
                                    UserOnProjectFrom = projectFrom2,
                                    UserOnProjectTo = projectTo2,
                                    ProjectId = project2Id,
                                    TotalBillableTrackedInMinutes = 0,
                                    TotalFactTrackedInMinutes = 0,
                                }
                            ],
                        }
                    ],
                }
            ],
            Projects =
                [
                    new() {
                        ProjectId = projectId,
                        ClientId = clientId,
                        Name = projectName,
                    },
                    new() {
                        ProjectId = project2Id,
                        ClientId = clientId,
                        Name = projectName2,
                    }
                ],
            Clients =
                [
                    new() {
                        ClientId = clientId,
                        Name = clientName,
                        Color = clientColor,
                    }
                ],
        };

        var ownExpenseAbsence = new List<UserInquiryDto>
        {
            new()
            {
                Id = 1,
                UserId = userId,
                InquiryTemplateName = "OwnExpense",
                InquiryType = InquiryType.OwnExpense,
                DateFrom = new DateOnly(2025, 2, 9),
                DateTo = new DateOnly(2025, 2, 12),
                Status = UserInquiryStatus.Approved
            }
        };

        _payloadUserDetailsLoaderMock
            .Setup(mock => mock.Load(It.IsAny<int[]>(), year, null, _cancellationToken))
            .ReturnsAsync(users);

        _timeTrackContractMock
            .Setup(mock => mock.GetTrackedTimePayloadForUsers(It.IsAny<int[]>(), year, null, null, _cancellationToken))
            .ReturnsAsync(payload);

        _inquiriesContractMock
            .Setup(mock => mock.GetUsersInquiriesAsync(It.IsAny<int[]>(), _cancellationToken, It.IsAny<InquiryType>(), It.IsAny<UserInquiryStatus>()))
            .ReturnsAsync(ownExpenseAbsence);

        _inquiriesContractMock
            .Setup(mock => mock.GetCalendarExclusionAsync(It.IsAny<int>(), _cancellationToken, null))
            .ReturnsAsync([]);

        _clockService
         .Setup(mock => mock.TodayUtc())
         .Returns(new DateOnly(2025, 3, 10));

        //Act
        var result = await _composer.Compose(year, userIds, userIds.Length, filter, _cancellationToken);

        //Assert
        result.Year.Should().Be(year);

        var project = result.Projects.First(project => project.ProjectId == projectId);

        project.Name.Should().Be(projectName);
        project.ClientId.Should().Be(clientId);

        project = result.Projects.First(project => project.ProjectId == project2Id);

        project.Name.Should().Be(projectName2);
        project.ClientId.Should().Be(clientId);

        foreach (var client in result.Clients)
        {
            client.ClientId.Should().Be(clientId);
            client.Name.Should().Be(clientName);
            client.Color.Should().Be(clientColor);
        }

        var user = result.Users.First();

        user.NeedProject.Should().Be(isUserNeedProject);
        user.Comments.Should().Be(userComment);
        user.FirstName.Should().Be(userFirstName);
        user.LastName.Should().Be(userLastName);
        user.Color.Should().Be(userColor);
        user.DepartmentId.Should().Be(userDepartment);
        user.SpecializationName.Should().Be(userPrimarySpecialization);

        user.Projects.First(x => x.ProjectId == projectId).IsActiveAssignement.Should().Be(isActiveProject);
        user.Projects.First(x => x.ProjectId == projectId).IsFutureActiveAssignment.Should().Be(isFutureActive);

        user.Projects.First(x => x.ProjectId == project2Id).IsActiveAssignement.Should().Be(isActiveProject2);
        user.Projects.First(x => x.ProjectId == project2Id).IsFutureActiveAssignment.Should().Be(isFutureActive2);

        user.HourDetails.Count.Should().Be(12);


        // check here

        foreach (var hourDetail in user.HourDetails)
        {
            if (hourDetail.MonthOfYear == 2)
            {
                hourDetail.ActualFte.Should().Be(0.06m);
                hourDetail.PlannedFte.Should().Be(0.5m);
                hourDetail.PlannedWorkloadInHours.Should().Be((_calendarDetails.CalendarWorkingHourDetails[1].CountryHours[0].Hours - 24) * 0.5m);
                hourDetail.ProjectDetails.Count.Should().Be(1);
                hourDetail.RequiredHours.Should().Be(59.25m);
                hourDetail.TrackedBillableInHours.Should().Be(10);

                var projectDetail = hourDetail.ProjectDetails.First();

                projectDetail.ProjectId.Should().Be(projectId);
                projectDetail.ActualFte.Should().Be(0.06m);
                projectDetail.TrackedBillableInHours.Should().Be(10);
                projectDetail.RequiredHours.Should().Be(59.25m);
            }
            else if (hourDetail.MonthOfYear == 3)
            {
                hourDetail.ActualFte.Should().Be(0.02m);
                hourDetail.PlannedFte.Should().Be(0.5m);
                hourDetail.PlannedWorkloadInHours.Should().Be(_calendarDetails.CalendarWorkingHourDetails[2].CountryHours[0].Hours * 0.5m);
                hourDetail.ProjectDetails.Count.Should().Be(1);
                hourDetail.RequiredHours.Should().Be(80m);
                hourDetail.TrackedBillableInHours.Should().Be(4);

                var projectDetail = hourDetail.ProjectDetails.First();

                projectDetail.ProjectId.Should().Be(projectId);
                projectDetail.ActualFte.Should().Be(0.02m);
                projectDetail.TrackedBillableInHours.Should().Be(4);
                projectDetail.RequiredHours.Should().Be(80m);
            }
            else if (hourDetail.MonthOfYear == 4)
            {
                hourDetail.ActualFte.Should().Be(0m);
                hourDetail.PlannedFte.Should().Be(0m);
                hourDetail.PlannedWorkloadInHours.Should().Be(0m);
                hourDetail.ProjectDetails.Count.Should().Be(1);
                hourDetail.RequiredHours.Should().Be(15.64m);
                hourDetail.TrackedBillableInHours.Should().Be(0);

                var projectDetail = hourDetail.ProjectDetails.First();

                projectDetail.ProjectId.Should().Be(projectId);
                projectDetail.ActualFte.Should().Be(0m);
                projectDetail.TrackedBillableInHours.Should().Be(0);
                projectDetail.RequiredHours.Should().Be(15.64m);
            }
            else if (hourDetail.MonthOfYear == 7)
            {
                hourDetail.ActualFte.Should().Be(0m);
                hourDetail.PlannedFte.Should().Be(0m);
                hourDetail.PlannedWorkloadInHours.Should().Be(0m);
                hourDetail.ProjectDetails.Count.Should().Be(1);
                hourDetail.RequiredHours.Should().Be(24m);
                hourDetail.TrackedBillableInHours.Should().Be(0);

                var projectDetail = hourDetail.ProjectDetails.First();

                projectDetail.ProjectId.Should().Be(project2Id);
                projectDetail.ActualFte.Should().Be(0m);
                projectDetail.TrackedBillableInHours.Should().Be(0);
                projectDetail.RequiredHours.Should().Be(24m);
            }
            else
            {
                hourDetail.ActualFte.Should().Be(0);
                hourDetail.PlannedFte.Should().Be(0);
                hourDetail.PlannedWorkloadInHours.Should().Be(0);
                hourDetail.ProjectDetails.Count.Should().Be(0);
                hourDetail.RequiredHours.Should().Be(0);
                hourDetail.TrackedBillableInHours.Should().Be(0);
            }
        }
    }

    [Fact]
    public async Task HandleWhenTwoAssignmentsInOneMonthShouldReturnCalculatedValue()
    {
        //Arrange
        var userIds = new[] { 1 };
        var year = 2025;
        var filter = new PayloadFilterCoreDto();

        var userId = 1;
        var userComment = "Comment";
        var userFirstName = "FirstName";
        var userLastName = "LastName";
        var userColor = "#ffffff";
        var userPrimarySpecialization = "userPrimarySpecialization";
        var userDepartment = ".Net";

        var projectId = 1;
        var projectName = "ProjectName";
        var projectFrom = new DateOnly(2025, 2, 10);
        var projectTo = new DateOnly(2025, 2, 13);

        var projectFrom2 = new DateOnly(2025, 2, 14);
        var projectTo2 = new DateOnly(2025, 2, 17);

        var isActiveProject = true;

        var clientId = 1;
        var clientName = "ClientName";
        var clientColor = "#ffffff";

        var users = new UserDetailsCoreDto[]
        {
            new()
            {
                UserId = userId,
                Comment = userComment,
                FirstName = userFirstName,
                LastName = userLastName,
                Color = userColor,
                PrimarySpecializationName = userPrimarySpecialization,
                DepartmentId = userDepartment,
                RegisteredLocations =
                [
                    new RegisteredLocationHistoryCoreDto
                    {
                        DateFrom = projectFrom.AddMonths(-1),
                        RegisteredLocationId = 1
                    }
                ],
                ProjectDetails =
                [
                    new() {
                        ProjectId = projectId,
                        Fte = 0.5m,
                        UserOnProjectFrom = projectFrom,
                        UserOnProjectTo = projectTo,
                        IsTimeOnProjectBillable = true,
                    },
                    new() {
                        ProjectId = projectId,
                        Fte = 1m,
                        UserOnProjectFrom = projectFrom2,
                        UserOnProjectTo = projectTo2,
                        IsTimeOnProjectBillable = true,
                    },
                ],
                UserMonthPayloadDetails =
                [
                    new() {
                        EstimatedFte = 0.5m,
                        PayloadMonthOfYear = 2,
                    },
                ],
            },
        };

        var payload = new PayloadContractDto
        {
            Year = year,
            Users =
            [
                new() {
                    UserId = userId,
                    HourDetails =
                    [
                        new() {
                            MonthOfYear = 2,
                            TotalBillableTrackedInMinutes = 11 * _minutesInHour,
                            TotalFactTrackedInMinutes = 11 * _minutesInHour,
                            Projects =
                            [
                                new() {
                                    IsActive = isActiveProject,
                                    UserOnProjectFrom = projectFrom,
                                    UserOnProjectTo = projectTo,
                                    ProjectId = projectId,
                                    TotalBillableTrackedInMinutes = 10 * _minutesInHour,
                                    TotalFactTrackedInMinutes = 10 * _minutesInHour,
                                },
                                new() {
                                    IsActive = isActiveProject,
                                    UserOnProjectFrom = projectFrom2,
                                    UserOnProjectTo = projectTo2,
                                    ProjectId = projectId,
                                    TotalBillableTrackedInMinutes = 1 * _minutesInHour,
                                    TotalFactTrackedInMinutes = 1 * _minutesInHour,
                                },
                            ],
                        },

                    ],
                }
            ],
            Projects =
                [
                    new() {
                        ProjectId = projectId,
                        ClientId = clientId,
                        Name = projectName,
                    }
                ],
            Clients =
                [
                    new() {
                        ClientId = clientId,
                        Name = clientName,
                        Color = clientColor,
                    }
                ],
        };

        _payloadUserDetailsLoaderMock
            .Setup(mock => mock.Load(It.IsAny<int[]>(), year, null, _cancellationToken))
            .ReturnsAsync(users);

        _timeTrackContractMock
            .Setup(mock => mock.GetTrackedTimePayloadForUsers(It.IsAny<int[]>(), year, null, null, _cancellationToken))
            .ReturnsAsync(payload);

        _inquiriesContractMock
            .Setup(mock => mock.GetUsersInquiriesAsync(It.IsAny<int[]>(), _cancellationToken, It.IsAny<InquiryType>(), It.IsAny<UserInquiryStatus>()))
            .ReturnsAsync([]);

        _inquiriesContractMock
            .Setup(mock => mock.GetCalendarExclusionAsync(It.IsAny<int>(), _cancellationToken, null))
            .ReturnsAsync([]);

        _clockService
            .Setup(mock => mock.TodayUtc())
            .Returns(new DateOnly(2025, 2, 17));

        //Act
        var result = await _composer.Compose(year, userIds, userIds.Length, filter, _cancellationToken);

        //Assert
        var hourDetail = result.Users.First().HourDetails.First(month => month.MonthOfYear == 2);

        hourDetail.ActualFte.Should().Be(0.06m);
        hourDetail.PlannedFte.Should().Be(0.5m);
        hourDetail.PlannedWorkloadInHours.Should().Be(_calendarDetails.CalendarWorkingHourDetails[1].CountryHours[0].Hours * 0.5m);
        hourDetail.ProjectDetails.Count.Should().Be(1);
        hourDetail.RequiredHours.Should().Be(36.40m);
        hourDetail.TrackedBillableInHours.Should().Be(11);

        var projectDetail = hourDetail.ProjectDetails.First();

        projectDetail.ProjectId.Should().Be(projectId);
        projectDetail.ActualFte.Should().Be(0.06m);
        projectDetail.PlannedFte.Should().Be(0.2m);
        projectDetail.TrackedBillableInHours.Should().Be(11m);
        projectDetail.RequiredHours.Should().Be(36.40m);
    }
}
