using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.PersonalInfo;

public class UpdatePersonalDataRequestTests
{
    private readonly Mock<IIsTeamBoardUserExistWithHRAccessControlDbQuery> _isTeamBoardUserExistingByUserIdDbQueryMock = new();
    private readonly Mock<ITeamBoardDbContext> _teamboardDbContextMock = new();
    private readonly Mock<IIdentityCrossWriterContract> _identityContractMock = new();
    private readonly Mock<IArePersonalPhoneAndEmailsUniqueBusinessRule> _arePersonalPhoneAndEmailsUniqueBusinessRule = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly UpdatePersonalDataRequest _handler;

    public UpdatePersonalDataRequestTests()
    {
        _handler = new UpdatePersonalDataRequest(
            _isTeamBoardUserExistingByUserIdDbQueryMock.Object,
            _teamboardDbContextMock.Object,
            _identityContractMock.Object,
            _arePersonalPhoneAndEmailsUniqueBusinessRule.Object);
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var userId = 999999;

        var query = new UpdatePersonalDataRequest.Command(
            new UpdatePersonalDataRequestDto(userId, null, null, null, null, null,
            null, null, null, null, null, null, null, null, "A1", new DateOnly(), null, null, null));

        _isTeamBoardUserExistingByUserIdDbQueryMock
            .Setup(x => x.ExecuteAsync(userId, _token))
            .ReturnsAsync(false);

        // Act
        var action = () => _handler.Handle(query, _token);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>("User is not found.");
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserExistsShouldReturnPersonalData()
    {
        // Arrange
        var userId = 15;
        var corporateEmail = new string("qwert1yyyy@modsen-software.com");
        var personalEmail = new string("qwert1yyyy@modsen.by");
        var personalEmailSecond = new string("qwertyyyy@modsen.by");
        var personalPhone = new string("375291111111");
        var telegram = "@testTg";
        var englishLevelId = new string("A1");
        var dateOfBirth = new DateOnly(2004, 1, 1);
        var firstName = "test";
        var lastName = "test";
        var firstNameRu = "тест";
        var lastNameRu = "тест";
        var pathronymicRu = "тест";
        var groupId = 1;
        var departmentId = "Net";
        var position = "Employee";

        var command = new UpdatePersonalDataRequest.Command(new UpdatePersonalDataRequestDto(
            userId,
            firstName,
            lastName,
            firstNameRu,
            lastNameRu,
            pathronymicRu,
            groupId,
            departmentId,
            corporateEmail,
            personalEmail,
            personalEmailSecond,
            personalPhone,
            telegram,
            null,
            englishLevelId,
            dateOfBirth,
            position,
            null,
            null
            ));

        _arePersonalPhoneAndEmailsUniqueBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<ArePersonalPhoneAndEmailsUniqueBusinessModel>(), _token))
            .ReturnsAsync(BusinessRuleResult.Success());

        _isTeamBoardUserExistingByUserIdDbQueryMock
            .Setup(x => x.ExecuteAsync(userId, _token))
            .ReturnsAsync(true);

        _teamboardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>()
            {
                new()
                {
                    UserId = userId,
                    FirstName = "",
                    LastName = "",
                    Email = "",
                    AvatarColor = "",
                }
            }.BuildMockDbSet().Object);

        _teamboardDbContextMock.Setup(x => x.EnglishLevels)
            .Returns(new List<EnglishLevel>()
            {
                new()
                {
                    Id = englishLevelId,
                    Description = "test",
                    ShortName = "test",
                    ExternalName = "test",
                    InternalName = "test"
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        result.Should().NotBeNull();
        result.PersonalEmail.Should().Be(personalEmail);
        result.PersonalPhone.Should().Be(personalPhone);
        result.EnglishLevelId.Should().Be(englishLevelId);
        result.Telegram.Should().Be(telegram);
        result.Position.Should().Be(position);
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserPersonalEmailExistsShouldThrowEntityConflictException()
    {
        // Arrange
        var userId = 15;
        var corporateEmail = new string("qwert1yyyy@modsen-software.com");
        var personalEmail = new string("qwertyyyy@modsen.by");
        var personalEmailSecond = new string("qwertyyyy@modsen.by");
        var personalPhone = new string("375291111111");
        var telegram = "@testTg";
        var englishLevelId = new string("A1");
        var dateOfBirth = new DateOnly(2004, 1, 1);
        var firstName = "test";
        var lastName = "test";
        var firstNameRu = "тест";
        var lastNameRu = "тест";
        var pathronymicRu = "тест";
        var groupId = 1;
        var departmentId = "Net";
        var position = "Employee";

        var command = new UpdatePersonalDataRequest.Command(new UpdatePersonalDataRequestDto(
            userId,
            firstName,
            lastName,
            firstNameRu,
            lastNameRu,
            pathronymicRu,
            groupId,
            departmentId,
            corporateEmail,
            personalEmail,
            personalEmailSecond,
            personalPhone,
            telegram,
            null,
            englishLevelId,
            dateOfBirth,
            position,
            MilitaryServiceStatus.Fit,
            IsGraduatedJobPlacement: false));

        var failReason = "Reason";

        _arePersonalPhoneAndEmailsUniqueBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<ArePersonalPhoneAndEmailsUniqueBusinessModel>(), _token))
            .ReturnsAsync(BusinessRuleResult.Fail(failReason));

        _isTeamBoardUserExistingByUserIdDbQueryMock
            .Setup(x => x.ExecuteAsync(userId, _token))
            .ReturnsAsync(true);

        _teamboardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>()
            {
                new()
                {
                    UserId = userId + 1,
                    FirstName = "",
                    LastName = "",
                    Email = "",
                    PersonalEmail = personalEmail,
                    PersonalPhone = "sdfsdfds",
                    AvatarColor = "",
                },
                new()
                {
                    UserId = userId,
                    FirstName = "",
                    LastName = "",
                    Email = "",
                    PersonalEmail = "sfdsfdsf",
                    PersonalPhone = "safds",
                    AvatarColor = "",
                }
            }.BuildMockDbSet().Object);

        _teamboardDbContextMock.Setup(x => x.EnglishLevels)
            .Returns(new List<EnglishLevel>()
            {
                new()
                {
                    Id = englishLevelId,
                    Description = "test",
                    ShortName = "test",
                    ExternalName = "test",
                    InternalName = "test"
                }
            }.BuildMockDbSet().Object);

        // Act
        var action = () => _handler.Handle(command, _token);

        // Assert
        await action.Should().ThrowAsync<EntityConflictException>().WithMessage(failReason);
    }
}
