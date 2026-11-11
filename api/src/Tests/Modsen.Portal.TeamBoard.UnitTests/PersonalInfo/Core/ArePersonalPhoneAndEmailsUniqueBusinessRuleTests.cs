using MockQueryable.Moq;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.PersonalInfo.Core;

public class ArePersonalPhoneAndEmailsUniqueBusinessRuleTests
{
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
    private readonly ArePersonalPhoneAndEmailsUniqueBusinessRule _businessRule;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ArePersonalPhoneAndEmailsUniqueBusinessRuleTests()
    {
        _businessRule = new ArePersonalPhoneAndEmailsUniqueBusinessRule(_teamBoardDbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenEmailsDuplicatedInInputWithDifferentCaseShouldReturnFail()
    {
        // Arrange
        var model = new ArePersonalPhoneAndEmailsUniqueBusinessModel
        {
            UserId = 1,
            PersonalEmail = "TeSt@modsen.by",
            PersonalEmailSecond = "test@modsen.by"
        };

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(model, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("Personal emails are duplicated");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenPersonalPhoneExistsInDbReturnFail()
    {
        // Arrange
        var model = new ArePersonalPhoneAndEmailsUniqueBusinessModel
        {
            UserId = 1,
            PersonalPhone = "1234567"
        };

        var firstName = "John";
        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                {
                    UserId = 2,
                    FirstName = firstName,
                    LastName = "Doe",
                    Email = "test.user@modsen-software.com",
                    PersonalEmail = "TeSt@modsen.by",
                    PersonalPhone = "1234567"
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(model, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Contain(firstName);
    }
}
