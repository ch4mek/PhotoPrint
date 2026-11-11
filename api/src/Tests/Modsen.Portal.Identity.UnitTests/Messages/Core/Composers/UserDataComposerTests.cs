using Modsen.Portal.Identity.Requests.Messages.Core.Composers;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Messages.Core.Composers;

public sealed class UserDataComposerTests
{
    private readonly Mock<ITeamBoardContract> _teamBoardContractMock = new();

    [Fact]
    public async Task ComposeAsyncShouldReturnMergedPermanentAndImpermanentUserInfo()
    {
        // Arrange
        var email = "ivan.petrov@test.com";
        var cancellationToken = CancellationToken.None;

        var permanentInfo = new PermanentUserInfoOutput
        {
            FirstName = "Ivan",
            LastName = "Petrov",
            Email = email,
            Position = "Software Engineer",
            Departament = "Engineering",
            HireDate = new DateOnly(2024, 1, 10),
            Contracts =
            [
                new ContractOutput
                {
                    DateFrom = new DateOnly(2024, 1, 10),
                    DateTo = null
                }
            ],
            Managers =
            [
                new ManagerOutput
                {
                    FirstName = "ManagerFirstName",
                    LastName = "ManagerLastName",
                    Position = "Engineering Manager"
                }
            ]
        };

        var impermanentInfo = new ImpermanentUserInfoOutput
        {
            FuturePerformanceReviewDate = new DateTime(2026, 7, 1),
            OpenRequests =
            [
                new UserInquiryOutput
                {
                    InquiryTemplateName = "Vacation",
                    DateFrom = new DateOnly(2026, 8, 1),
                    DateTo = new DateOnly(2026, 8, 10)
                }
            ],
            BalanceDays =
            [
                new UserBalanceDaysOutput
                {
                    InquiryTemplateName = "Vacation",
                    Balance = 12.5m
                }
            ]
        };

        _teamBoardContractMock
            .Setup(x => x.GetPermanentUserInfo(email, cancellationToken))
            .ReturnsAsync(permanentInfo);

        _teamBoardContractMock
            .Setup(x => x.GetImpermanentUserInfo(email, cancellationToken))
            .ReturnsAsync(impermanentInfo);

        var composer = new UserDataComposer(_teamBoardContractMock.Object);

        // Act
        var result = await composer.ComposeAsync(email, cancellationToken);

        // Assert
        Assert.Equal("Ivan", result[nameof(PermanentUserInfoOutput.FirstName)]);
        Assert.Equal("Petrov", result[nameof(PermanentUserInfoOutput.LastName)]);
        Assert.Equal(email, result[nameof(PermanentUserInfoOutput.Email)]);
        Assert.Equal("Software Engineer", result[nameof(PermanentUserInfoOutput.Position)]);
        Assert.Equal("Engineering", result[nameof(PermanentUserInfoOutput.Departament)]);
        Assert.Equal(new DateOnly(2024, 1, 10), result[nameof(PermanentUserInfoOutput.HireDate)]);

        Assert.Equal(
            new DateTime(2026, 7, 1),
            result[nameof(ImpermanentUserInfoOutput.FuturePerformanceReviewDate)]);

        Assert.Same(permanentInfo.Contracts, result[nameof(PermanentUserInfoOutput.Contracts)]);
        Assert.Same(permanentInfo.Managers, result[nameof(PermanentUserInfoOutput.Managers)]);
        Assert.Same(impermanentInfo.OpenRequests, result[nameof(ImpermanentUserInfoOutput.OpenRequests)]);
        Assert.Same(impermanentInfo.BalanceDays, result[nameof(ImpermanentUserInfoOutput.BalanceDays)]);

        _teamBoardContractMock.Verify(
            x => x.GetPermanentUserInfo(email, cancellationToken),
            Times.Once);

        _teamBoardContractMock.Verify(
            x => x.GetImpermanentUserInfo(email, cancellationToken),
            Times.Once);

        _teamBoardContractMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void CreateDictionaryShouldMergePropertiesFromPermanentAndImpermanentInfo()
    {
        // Arrange
        var permanentInfo = new PermanentUserInfoOutput
        {
            FirstName = "Ivan",
            LastName = "Petrov",
            Email = "ivan.petrov@test.com",
            Position = "Software Engineer"
        };

        var impermanentInfo = new ImpermanentUserInfoOutput
        {
            FuturePerformanceReviewDate = new DateTime(2026, 7, 1)
        };

        // Act
        var result = UserDataComposer.CreateDictionary(permanentInfo, impermanentInfo);

        // Assert
        Assert.Equal("Ivan", result[nameof(PermanentUserInfoOutput.FirstName)]);
        Assert.Equal("Petrov", result[nameof(PermanentUserInfoOutput.LastName)]);
        Assert.Equal("ivan.petrov@test.com", result[nameof(PermanentUserInfoOutput.Email)]);
        Assert.Equal("Software Engineer", result[nameof(PermanentUserInfoOutput.Position)]);
        Assert.Equal(
            new DateTime(2026, 7, 1),
            result[nameof(ImpermanentUserInfoOutput.FuturePerformanceReviewDate)]);
    }


    [Fact]
    public void CreateDictionaryShouldReturnEmptyDictionaryWhenAllObjectsAreNull()
    {
        // Act
        var result = UserDataComposer.CreateDictionary(null, null);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void CreateDictionaryShouldNotKeepNullPropertyValues()
    {
        // Arrange
        var permanentInfo = new PermanentUserInfoOutput
        {
            FirstName = null,
            LastName = "Petrov",
            Email = null
        };

        // Act
        var result = UserDataComposer.CreateDictionary(permanentInfo);

        // Assert
        Assert.False(result.ContainsKey(nameof(PermanentUserInfoOutput.FirstName)));
        Assert.False(result.ContainsKey(nameof(PermanentUserInfoOutput.Email)));

        Assert.Equal("Petrov", result[nameof(PermanentUserInfoOutput.LastName)]);
    }

    [Fact]
    public void CreateDictionaryShouldKeepCollectionProperties()
    {
        // Arrange
        var contracts = new List<ContractOutput>
        {
            new()
            {
                DateFrom = new DateOnly(2024, 1, 1),
                DateTo = null
            }
        };

        var managers = new List<ManagerOutput>
        {
            new()
            {
                FirstName = "ManagerFirstName",
                LastName = "ManagerLastName",
                Position = "Manager"
            }
        };

        var permanentInfo = new PermanentUserInfoOutput
        {
            Contracts = contracts,
            Managers = managers
        };

        // Act
        var result = UserDataComposer.CreateDictionary(permanentInfo);

        // Assert
        Assert.Same(contracts, result[nameof(PermanentUserInfoOutput.Contracts)]);
        Assert.Same(managers, result[nameof(PermanentUserInfoOutput.Managers)]);
    }
}
