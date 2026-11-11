using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Authentication.Jwt;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.IdentityPortalSettings;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class ExternalLoginCallbackRequestTests
{
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IAuthenticationTokenContext> _authenticatedTokenContext = new();
    private readonly Mock<IIsApplicationRolesCorrectBusinessRule> _isApplicationRolesCorrectBusinessRuleMock = new();
    private readonly Mock<IHostEnvironmentContext> _hostEnvContextMock = new();
    private readonly Mock<IIdentityDbContext> _dbContext = new();
    private readonly Mock<IIdentityPortalSettingsAccessor> _settingsAccessor = new();

    private readonly ExternalLoginCallbackRequest _handler;

    private const string _returnUrl = "returnUrl";
    private static readonly TokenModel _defaultToken = new("token", DateTime.UtcNow);
    private readonly string _validEmail = "Test@modsen-software.com";

    public ExternalLoginCallbackRequestTests()
    {
        _handler = new ExternalLoginCallbackRequest(
            _tokenServiceMock.Object,
            _hostEnvContextMock.Object,
            _isApplicationRolesCorrectBusinessRuleMock.Object,
            _settingsAccessor.Object,
            _authenticatedTokenContext.Object,
            _dbContext.Object
        );

        _hostEnvContextMock.Setup(x => x.IsProductionEnvironment()).Returns(true);

        _settingsAccessor
            .Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdentityPortalSettingsDto
            {
                MaxCeoCount = 1,
                MaxCtoCount = 1,
                MaxHeadOfSpecificDepartmentCount = 10,
                DefaultTimeZone = "BY",
                DefaultColorHex = "#BDBDBD",
                EmailDomain = "@modsen-software.com"
            });
    }

    [Fact]
    public async Task ExternalLoginCallbackWhenAuthenticateResultFailShouldReturnErrorUrl()
    {
        //Arrange
        var query = new ExternalLoginCallbackRequest.Command(
            new AuthenticationResult
            {
                Succeeded = false,
                ReturnUrl = _returnUrl
            });

        var expectedResult = new ExternalLoginDto(false, "External authentication error");

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ExternalLoginCallbackWhenUserEmailIsNotInModsenDomainShouldReturnErrorUrl()
    {
        //Arrange
        _dbContext.Setup(x => x.Users)
            .Returns(new List<User>
            {
                new()
                {
                    Id = 1,
                    Email = _validEmail + "invalid",
                    NormalizedEmail = (_validEmail + "invalid").ToUpper().Normalize(),
                    DepartmentId = "NET",
                    FirstName = "TestGivenName",
                    LastName = "TestSurname",
                    IsActive = true,
                    Location = LocationType.Minsk,
                    UserRoles =
                    [
                        new() { Role = new Role { Name = "Employee", OrderWeight = 100, Claims = [], PublicName = "Test" } }
                    ],
                    Claims = []
                }
            }.BuildMockDbSet().Object);

        var expectedResult = new ExternalLoginDto(false, "User is not in modsen-software.com domain");

        //Act
        var result = await _handler.Handle(new ExternalLoginCallbackRequest.Command(new AuthenticationResult
        {
            Succeeded = true,
            Email = _validEmail + "invalid",
            ReturnUrl = _returnUrl
        }), CancellationToken.None);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ExternalLoginCallbackWhenApplicationRolesIncorrectShouldReturnErrorUrlAndTerminateSessions()
    {
        //Arrange
        _dbContext.Setup(x => x.Users)
            .Returns(new List<User>
            {
                new()
                {
                    Id = 1,
                    Email = _validEmail,
                    DepartmentId = "NET",
                    FirstName = "TestGivenName",
                    LastName = "TestSurname",
                    IsActive = true,
                    Location = LocationType.Minsk,
                    UserRoles = new List<UserRole>
                    {
                        new() { Role = new Role { Name = "Employee", OrderWeight = 100, Claims = [], PublicName = "Test" } }
                    },
                    Claims = []
                }
            }.BuildMockDbSet().Object);

        _isApplicationRolesCorrectBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(null, CancellationToken.None))
            .ReturnsAsync(false);

        _authenticatedTokenContext
            .Setup(x => x.RegisterUserToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _dbContext
            .Setup(x => x.Roles)
            .Returns(new List<Role>{
                new() {
                    Name = PortalRoleNames.Employee,
                    Id = 1,
                    PublicName = "Test"
                }
            }.BuildMockDbSet().Object);

        var expectedResult = new ExternalLoginDto(false, "Internal authentication error");

        //Act
        var result = await _handler.Handle(new ExternalLoginCallbackRequest.Command(new AuthenticationResult
        {
            Succeeded = true,
            Email = _validEmail,
            GivenName = "TestGivenName",
            Surname = "TestSurname",
            ReturnUrl = _returnUrl
        }), CancellationToken.None);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ExternalLoginCallbackWhenUserIsNotActiveShouldReturnErrorUrl()
    {
        //Arrange
        _dbContext.Setup(x => x.Users)
            .Returns(new List<User>
            {
                new()
                {
                    Id = 1,
                    Email = _validEmail,
                    DepartmentId = "NET",
                    FirstName = "TestGivenName",
                    LastName = "TestSurname",
                    IsActive = false,
                    Location = LocationType.Minsk,
                    UserRoles = new List<UserRole>
                    {
                        new() { Role = new Role { Name = "Employee", OrderWeight = 100, Claims = [], PublicName = "Test" } }
                    },
                    Claims = []
                }
            }.BuildMockDbSet().Object);

        _isApplicationRolesCorrectBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(null, CancellationToken.None))
            .ReturnsAsync(true);

        var expectedResult = new ExternalLoginDto(false, "User is inactive");

        //Act
        var result = await _handler.Handle(new ExternalLoginCallbackRequest.Command(new AuthenticationResult
        {
            Succeeded = true,
            Email = _validEmail,
            GivenName = "TestGivenName",
            Surname = "TestSurname",
            ReturnUrl = _returnUrl
        }), CancellationToken.None);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ExternalLoginCallbackWhenUserIsActiveShouldReturnValidExternalLoginDto()
    {
        //Arrange
        _dbContext.Setup(x => x.Users)
            .Returns(new List<User>
            {
                new()
                {
                    Id = 1,
                    Email = _validEmail,
                    DepartmentId = "NET",
                    FirstName = "TestGivenName",
                    LastName = "TestSurname",
                    IsActive = true,
                    Location = LocationType.Minsk,
                    UserRoles = new List<UserRole>
                    {
                        new() { Role = new Role { Name = "Employee", OrderWeight = 100, Claims = [], PublicName = "Test" } }
                    },
                    Claims = []
                }
            }.BuildMockDbSet().Object);

        _dbContext.Setup(x => x.PermissionAssignments)
            .Returns(new List<PermissionAssignment>().BuildMockDbSet().Object);

        _isApplicationRolesCorrectBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(null, CancellationToken.None))
            .ReturnsAsync(true);

        _tokenServiceMock
            .Setup(x => x.GenerateToken(It.IsAny<UserTokenDto>()))
            .Returns(_defaultToken);
        
        var expectedResult =
            new ExternalLoginDto(true, Token: _defaultToken);

        //Act
        var result = await _handler.Handle(new ExternalLoginCallbackRequest.Command(new AuthenticationResult
        {
            Succeeded = true,
            Email = _validEmail,
            GivenName = "TestGivenName",
            Surname = "TestSurname",
            ReturnUrl = _returnUrl
        }), CancellationToken.None);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
