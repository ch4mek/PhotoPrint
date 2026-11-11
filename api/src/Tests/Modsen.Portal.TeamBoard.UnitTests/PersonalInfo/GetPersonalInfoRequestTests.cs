using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Resolvers;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings.CoreDto;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.PersonalInfo;

public class GetPersonalInfoRequestTests
{
    private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _mockBusinessRule = new();
    private readonly Mock<ISpecializationSeniorityResolver> _specializationSeniorityResolver = new();
    private readonly Mock<IPerformanceReviewDateTypeCalculator> _reviewDateTypeCalculator = new();
    private readonly Mock<ITeamBoardPortalSettingsAccessor> _settingsAccessor = new();
    private readonly Mock<IUserPersonalInfoLoader> _personalInfoLoaderMock = new();
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly GetPersonalInfoRequest _request;

    public GetPersonalInfoRequestTests()
    {
        _request = new GetPersonalInfoRequest(
            _mockBusinessRule.Object,
            _settingsAccessor.Object,
            _specializationSeniorityResolver.Object,
            _personalInfoLoaderMock.Object,
            _teamBoardDbContextMock.Object,
            _reviewDateTypeCalculator.Object,
            _authenticatedUserContextMock.Object);

        _settingsAccessor
            .Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamBoardPortalSettingsDto
            {
                SystemAdministrationDepartmentId = "SysAdmin",
                OverdueDaysCount = 7,
                UpcomingIminentBorderDaysCount = 3
            });
    }

    [Fact]
    public async Task HandleWhenBusinessRuleIsSuccessfulShouldReturnPersonalInfo()
    {
        // Arrange
        var query = new GetPersonalInfoRequest.Query(1, null);
        var cancellationToken = CancellationToken.None;

        var userSoftSkills = new List<UserSoftSkillCoreDto>
        {
            new(3, "MiddleL3", 200, 2, true),
            new(5, "JuniorL2", 50, 1, false)
        };

        var accessOptions = AccessOptions.Self | AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;
        _mockBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), accessOptions, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var personalInfo = new PersonalInfoCoreDto(FullName: "John Doe",
            FullNameRu: "Джон Доу",
            FirstName: "John",
            LastName: "Doe",
            FirstNameRu: "Джон",
            LastNameRu: "Доу",
            PatronymicRu: null,
            Email: "john.doe@example.com",
            PersonalEmail: "johndoe@gmail.com",
            PersonalEmailSecond: "johndoe@gmail.com",
            PersonalPhone: "1234567890",
            Telegram: "@johnDoe",
            LinkedIn: null,
            Position: "Employee",
            DateOfBirth: DateOnly.FromDateTime(DateTime.Today.AddYears(1)),
            Location: "Minsk",
            SeniorityLevel: "Junior+",
            SeniorityLevelCoefficient: 1,
            PrimarySpecialization: "Backend Developer",
            DepartmentId: "Net",
            DepartmentFullName: ".NET",
            ResourceGroupId: 1,
            ResourceGroupName: "Group 1",
            EnglishLevel: "Advanced",
            Comments: "No comments",
            Color: "Blue",
            AvatarFileId: "file123",
            NeedProject: false,
            UserSoftSkillsSnapshots: new List<UserSoftSkillsSnapshotCoreDto>()
            {
                new(
                userSoftSkills, DateOnly.FromDateTime(DateTime.Now), 1)
            },
            null, null,
            RegisteredLocation: "Belarus",
            RegisteredLocationId: 1,
            CurrentLocation: "Minsk",
            MilitaryServiceStatus: MilitaryServiceStatus.Unfit,
            IsGraduateJobPlacement: true);

        _personalInfoLoaderMock.Setup(x => x.LoadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(personalInfo);
        var softSkillAverageSeniority = "Junior+";
        var generalSeniority = "Junior";

        _specializationSeniorityResolver.Setup(
                x => x.ResolveAsync(It.IsAny<HardSkillSeniorityDto[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(softSkillAverageSeniority);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(["CEO"]);

        _teamBoardDbContextMock.Setup(x => x.SeniorityLevels)
            .Returns(new List<SeniorityLevel>
            {
                new()
                {
                    Id = "Junior",
                    ShortName = "Junior",
                    InternalName = "Junior",
                    Description = "Junior",
                    Coefficient = 1,
                    ExternalName = "Junior"
                },
                new()
                {
                    ShortName = "Junior+",
                    Description = "Junior+",
                    Id = "Junior+",
                    InternalName = "Junior+",
                    ExternalName = "Junior+",
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = await _request.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.FullName.Should().Be(personalInfo.FullName);
        result.FullNameRu.Should().Be(personalInfo.FullNameRu);
        result.Email.Should().Be(personalInfo.Email);
        result.PersonalEmail.Should().Be(personalInfo.PersonalEmail);
        result.PersonalPhone.Should().Be(personalInfo.PersonalPhone);
        result.Location.Should().Be(personalInfo.Location);
        result.Seniority.Should().Be(personalInfo.SeniorityLevel);
        result.PrimarySpecialization.Should().Be(personalInfo.PrimarySpecialization);
        result.EnglishLevel.Should().Be(personalInfo.EnglishLevel);
        result.Comments.Should().Be(personalInfo.Comments);
        result.Color.Should().Be(personalInfo.Color);
        result.AvatarFileId.Should().Be(personalInfo.AvatarFileId);
        result.SoftSkillAverageSeniority.Should().Be(softSkillAverageSeniority);
        result.GeneralSeniority.Should().Be(generalSeniority);
        result.DateOfBirth.Should().Be(personalInfo.DateOfBirth);
        result.RegisteredLocation.Should().Be(personalInfo.RegisteredLocation);
        result.Telegram.Should().BeEquivalentTo(personalInfo.Telegram);
        result.NeedProject.Should().Be(personalInfo.NeedProject);
    }

    [Fact]
    public async Task HandleWhenUserIsNotManagerShouldReturnPersonalInfo()
    {
        // Arrange
        var query = new GetPersonalInfoRequest.Query(1, null);
        var cancellationToken = CancellationToken.None;

        var userSoftSkills = new List<UserSoftSkillCoreDto>
        {
            new(3, "MiddleL3", 200, 2, true),
            new(5, "JuniorL2", 50, 1, false)
        };

        var accessOptions = AccessOptions.Self | AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;
        _mockBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), accessOptions, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var personalInfo = new PersonalInfoCoreDto(FullName: "John Doe",
            FullNameRu: "Джон Доу",
            FirstName: "John",
            LastName: "Doe",
            FirstNameRu: "Джон",
            LastNameRu: "Доу",
            PatronymicRu: null,
            Email: "john.doe@example.com",
            PersonalEmail: "johndoe@gmail.com",
            PersonalEmailSecond: "johndoe@gmail.com",
            PersonalPhone: "1234567890",
            Telegram: "@johnDoe",
            LinkedIn: null,
            Position: "Employee",
            DateOfBirth: DateOnly.FromDateTime(DateTime.Today.AddYears(1)),
            Location: "Minsk",
            SeniorityLevel: "Junior+",
            SeniorityLevelCoefficient: 1,
            PrimarySpecialization: "Backend Developer",
            DepartmentId: "Net",
            DepartmentFullName: ".NET",
            ResourceGroupId: 1,
            ResourceGroupName: "Group 1",
            EnglishLevel: "Advanced",
            Comments: "No comments",
            Color: "Blue",
            AvatarFileId: "file123",
            NeedProject: false,
            UserSoftSkillsSnapshots: new List<UserSoftSkillsSnapshotCoreDto>()
            {
                new(
                userSoftSkills, DateOnly.FromDateTime(DateTime.Now), 1)
            },
            null, null,
            RegisteredLocation: "Belarus",
            RegisteredLocationId: 1,
            CurrentLocation: "Minsk",
            MilitaryServiceStatus.Unfit,
            IsGraduateJobPlacement: true
        );

        _personalInfoLoaderMock.Setup(x => x.LoadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(personalInfo);
        var softSkillAverageSeniority = "Junior+";

        _specializationSeniorityResolver.Setup(
                x => x.ResolveAsync(It.IsAny<HardSkillSeniorityDto[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(softSkillAverageSeniority);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(["Employee"]);

        _teamBoardDbContextMock.Setup(x => x.SeniorityLevels)
            .Returns(new List<SeniorityLevel>
            {
                new()
                {
                    Id = "Junior",
                    ShortName = "Junior",
                    InternalName = "Junior",
                    Description = "Junior",
                    Coefficient = 1,
                    ExternalName = "Junior"
                },
                new()
                {
                    ShortName = "Junior+",
                    Description = "Junior+",
                    Id = "Junior+",
                    InternalName = "Junior+",
                    ExternalName = "Junior+",
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = await _request.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.FullName.Should().Be(personalInfo.FullName);
        result.FullNameRu.Should().Be(personalInfo.FullNameRu);
        result.Email.Should().Be(personalInfo.Email);
        result.PersonalEmail.Should().Be(personalInfo.PersonalEmail);
        result.PersonalPhone.Should().Be(personalInfo.PersonalPhone);
        result.Location.Should().Be(personalInfo.Location);
        result.Seniority.Should().Be(null);
        result.PrimarySpecialization.Should().Be(null);
        result.EnglishLevel.Should().Be(personalInfo.EnglishLevel);
        result.Comments.Should().Be(null);
        result.Color.Should().Be(personalInfo.Color);
        result.AvatarFileId.Should().Be(personalInfo.AvatarFileId);
        result.SoftSkillAverageSeniority.Should().Be(null);
        result.GeneralSeniority.Should().Be(null);
        result.DateOfBirth.Should().Be(personalInfo.DateOfBirth);
        result.RegisteredLocation.Should().Be(personalInfo.RegisteredLocation);
        result.Telegram.Should().BeEquivalentTo(personalInfo.Telegram);
        result.NeedProject.Should().Be(null);
    }

    [Fact]
    public async Task HandleWhenUserICurrentUserShouldReturnPersonalInfo()
    {
        // Arrange
        var query = new GetPersonalInfoRequest.Query(1, null);
        var cancellationToken = CancellationToken.None;

        var userSoftSkills = new List<UserSoftSkillCoreDto>
        {
            new(3, "MiddleL3", 200, 2, true),
            new(5, "JuniorL2", 50, 1, false)
        };

        var accessOptions = AccessOptions.Self | AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;
        _mockBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), accessOptions, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var personalInfo = new PersonalInfoCoreDto(FullName: "John Doe",
            FullNameRu: "Джон Доу",
            FirstName: "John",
            LastName: "Doe",
            FirstNameRu: "Джон",
            LastNameRu: "Доу",
            PatronymicRu: null,
            Email: "john.doe@example.com",
            PersonalEmail: "johndoe@gmail.com",
            PersonalEmailSecond: "johndoe@gmail.com",
            PersonalPhone: "1234567890",
            Telegram: "@johnDoe",
            LinkedIn: null,
            Position: "Employee",
            DateOfBirth: DateOnly.FromDateTime(DateTime.Today.AddYears(1)),
            Location: "Minsk",
            SeniorityLevel: "Junior+",
            SeniorityLevelCoefficient: 1,
            PrimarySpecialization: "Backend Developer",
            DepartmentId: "Net",
            DepartmentFullName: ".NET",
            ResourceGroupId: 1,
            ResourceGroupName: "Group 1",
            EnglishLevel: "Advanced",
            Comments: "No comments",
            Color: "Blue",
            AvatarFileId: "file123",
            NeedProject: false,
            UserSoftSkillsSnapshots: new List<UserSoftSkillsSnapshotCoreDto>()
            {
                new(
                userSoftSkills, DateOnly.FromDateTime(DateTime.Now), 1)
            },
            null, null,
            RegisteredLocation: "Belarus",
            RegisteredLocationId: 1,
            CurrentLocation: "Minsk",
            MilitaryServiceStatus.Unfit,
            IsGraduateJobPlacement: true
        );

        _personalInfoLoaderMock.Setup(x => x.LoadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(personalInfo);
        var softSkillAverageSeniority = "Junior+";
        var generalSeniority = "Junior";

        _specializationSeniorityResolver.Setup(
                x => x.ResolveAsync(It.IsAny<HardSkillSeniorityDto[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(softSkillAverageSeniority);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(["Employee"]);
        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(1);

        _teamBoardDbContextMock.Setup(x => x.SeniorityLevels)
            .Returns(new List<SeniorityLevel>
            {
                new()
                {
                    Id = "Junior",
                    ShortName = "Junior",
                    InternalName = "Junior",
                    Description = "Junior",
                    Coefficient = 1,
                    ExternalName = "Junior"
                },
                new()
                {
                    ShortName = "Junior+",
                    Description = "Junior+",
                    Id = "Junior+",
                    InternalName = "Junior+",
                    ExternalName = "Junior+",
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = await _request.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.FullName.Should().Be(personalInfo.FullName);
        result.FullNameRu.Should().Be(personalInfo.FullNameRu);
        result.Email.Should().Be(personalInfo.Email);
        result.PersonalEmail.Should().Be(personalInfo.PersonalEmail);
        result.PersonalPhone.Should().Be(personalInfo.PersonalPhone);
        result.Location.Should().Be(personalInfo.Location);
        result.Seniority.Should().Be(personalInfo.SeniorityLevel);
        result.PrimarySpecialization.Should().Be(personalInfo.PrimarySpecialization);
        result.EnglishLevel.Should().Be(personalInfo.EnglishLevel);
        result.Comments.Should().Be(null);
        result.Color.Should().Be(personalInfo.Color);
        result.AvatarFileId.Should().Be(personalInfo.AvatarFileId);
        result.SoftSkillAverageSeniority.Should().Be(softSkillAverageSeniority);
        result.GeneralSeniority.Should().Be(generalSeniority);
        result.DateOfBirth.Should().Be(personalInfo.DateOfBirth);
        result.RegisteredLocation.Should().Be(personalInfo.RegisteredLocation);
        result.Telegram.Should().BeEquivalentTo(personalInfo.Telegram);
        result.NeedProject.Should().Be(null);
    }

    [Fact]
    public async Task HandleShouldThrowNotFoundExceptionWhenBusinessRuleFails()
    {
        // Arrange
        var query = new GetPersonalInfoRequest.Query(1, null);
        var cancellationToken = CancellationToken.None;

        _mockBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ActionNotAllowedException>(() =>
        _request.Handle(query, cancellationToken));
    }
}
