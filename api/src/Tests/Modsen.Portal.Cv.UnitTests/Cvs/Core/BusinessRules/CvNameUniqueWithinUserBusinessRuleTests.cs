using MockQueryable.Moq;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Cvs.Core.BusinessRules;

namespace Modsen.Portal.Cv.UnitTests.Cvs.Core.BusinessRules;

public class CvNameUniqueWithinUserBusinessRuleTests
{
    private readonly Mock<ICvDbContext> _cvDbContextMock;
    private readonly CvNameUniqueWithinUserBusinessRule _businessRule;
    private readonly CancellationToken _cancellationToken;

    public CvNameUniqueWithinUserBusinessRuleTests()
    {
        _cvDbContextMock = new Mock<ICvDbContext>();
        _businessRule = new CvNameUniqueWithinUserBusinessRule(_cvDbContextMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCvNameIsUniqueReturnsTrue()
    {
        // Arrange
        const string cvName = "MyCV";
        const int userId = 1;
        var cvs = new List<CvEntity>
        {
            new()
            {
                Id = 1,
                CvName = "OtherCV",
                UserId = userId,
                IsDeleted = false,
                UserFirstName = "test",
                UserLastName = "test",
            }
        };
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(cvName, userId, null, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCvNameExistsForUserReturnsFalse()
    {
        // Arrange
        const string cvName = "MyCV";
        const int userId = 1;
        var cvs = new List<CvEntity>
        {
            new()
            {
                Id = 1,
                CvName = cvName,
                UserId = userId,
                IsDeleted = false,
                UserFirstName = "test",
                UserLastName = "test",
            }
        };
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(cvName, userId, null, _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCvNameExistsForDifferentUserReturnsTrue()
    {
        // Arrange
        const string cvName = "MyCV";
        const int userId = 1;
        const int otherUserId = 2;
        var cvs = new List<CvEntity>
        {
            new()
            {
                Id = 1,
                CvName = cvName,
                UserId = otherUserId,
                IsDeleted = false,
                UserFirstName = "test",
                UserLastName = "test",
            }
        };
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(cvName, userId, null, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenNoCvsExistReturnsTrue()
    {
        // Arrange
        const string cvName = "MyCV";
        const int userId = 1;
        var cvs = new List<CvEntity>();
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(cvName, userId, null, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }
}
