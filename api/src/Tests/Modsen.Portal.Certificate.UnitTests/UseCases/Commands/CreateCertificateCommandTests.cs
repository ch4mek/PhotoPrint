using Modsen.Portal.Certificate.UseCases.CertificatePortalSettings;
using Modsen.Portal.Certificate.UseCases.Dto;
using Modsen.Portal.Certificate.UseCases.Requests;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Certificate;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.ModsenApi.Contract;
using Modsen.Portal.TeamBoard.Contract;
using Moq;

namespace Modsen.Portal.Certificate.UnitTests.UseCases.Commands;

public class CreateCertificateCommandTests
{
    private readonly CreateCertificateCommandUseCase _handler;

    private readonly Mock<IModsenApiCertificatesClient> _mockCertificatesClient = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly Mock<ITeamBoardContract> _teamBoardContract = new();
    private readonly Mock<ICertificatePortalSettingsAccessor> _settingsAccessorMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateCertificateCommandTests()
    {
        _handler = new CreateCertificateCommandUseCase(_identityContractMock.Object,
            _teamBoardContract.Object,
            _mockCertificatesClient.Object,
            _settingsAccessorMock.Object);

        _settingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CertificatePortalSettingsDto
            {
                MonthDayYearFormat = "MM.dd.yyyy",
                MonthYearFormat = "MMM yyyy"
            });
    }

    [Fact]
    public async Task HandleWhenUserIdIsNotANumberThrowInvalidDataApp()
    {
        // Arrange
        var request = new CreateCertificateCommandUseCase.Command(new CreateCertificateDto
        {
            ReceiptDate = DateTime.UtcNow,
            Recipient = "Recipient",
            RecipientId = "NaN",
            Stack = JsStackType.React,
            Technology = "Js"
        });

        // Act
        var act = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidDataAppException>();
    }

    [Fact]
    public async Task HandleWhenUserIsNotEmployeeThrowActionNotAllowed()
    {
        // Arrange
        var request = new CreateCertificateCommandUseCase.Command(new CreateCertificateDto
        {
            ReceiptDate = DateTime.UtcNow,
            Recipient = "Recipient",
            RecipientId = "555",
            Stack = JsStackType.React,
            Technology = "Js"
        });

        var userId = 555;

        _identityContractMock.Setup(x => x.UserRoles(userId, _cancellationToken))
            .ReturnsAsync([PortalRoleNames.HeadOfDepartment]);

        // Act
        var act = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenUserIsNotAccessibleByCurrentUserThrowActionNotAllowed()
    {
        // Arrange
        var request = new CreateCertificateCommandUseCase.Command(new CreateCertificateDto
        {
            ReceiptDate = DateTime.UtcNow,
            Recipient = "Recipient",
            RecipientId = "555",
            Stack = JsStackType.React,
            Technology = "Js"
        });
        var userId = 555;

        _identityContractMock.Setup(x => x.UserRoles(userId, _cancellationToken))
            .ReturnsAsync([PortalRoleNames.Employee]);

        var teamBoardResponse = new Dictionary<int, bool>
        {
            [userId] = false
        };

        _teamBoardContract
            .Setup(x => x.CanUsersBeRetrievedByUser(new[] { userId }, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(teamBoardResponse);

        // Act
        var act = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>()
            .WithMessage($"You cannot create a certificate for user {userId}");
    }

    [Fact]
    public async Task HandleWhenUserIsNotAccessibleByCurrentUserEmptyDictThrowActionNotAllowed()
    {
        // Arrange
        var request = new CreateCertificateCommandUseCase.Command(new CreateCertificateDto
        {
            ReceiptDate = DateTime.UtcNow,
            Recipient = "Recipient",
            RecipientId = "555",
            Stack = JsStackType.React,
            Technology = "Js"
        });

        var userId = 555;

        _identityContractMock.Setup(x => x.UserRoles(userId, _cancellationToken))
            .ReturnsAsync([PortalRoleNames.Employee]);

        var teamBoardResponse = new Dictionary<int, bool>();

        _teamBoardContract
            .Setup(x => x.CanUsersBeRetrievedByUser(new[] { userId }, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(teamBoardResponse);

        // Act
        var act = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>()
            .WithMessage($"You cannot create a certificate for user {userId}");
    }

    [Fact]
    public async Task CreateCertificateWhenRequestIsValidCreateAndReturnNewCertificate()
    {
        //Arrange
        var request = new CreateCertificateCommandUseCase.Command(new CreateCertificateDto
        {
            ReceiptDate = DateTime.UtcNow,
            Recipient = "Recipient",
            RecipientId = "555",
            Stack = JsStackType.React,
            Technology = "Js"
        });

        var createCertificateModel = new CreateCertificateModel
        {
            ReceiptDate = request.Model.ReceiptDate,
            Recipient = request.Model.Recipient,
            RecipientId = request.Model.RecipientId,
            Stack = request.Model.Stack.ToString(),
            Technology = "Js"
        };

        var certificateModel = new CertificateModel
        {
            CertificateId = "123",
            ReceiptDate = createCertificateModel.ReceiptDate,
            Recipient = createCertificateModel.RecipientId,
            Stack = createCertificateModel.Stack,
            Technology = "Js",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            RecipientId = createCertificateModel.RecipientId,
        };

        var certificateDto = new CertificateDto
        {
            CertificateId = certificateModel.CertificateId,
            CreatedAt = certificateModel.CreatedAt,
            ReceiptDate = certificateModel.ReceiptDate,
            Recipient = certificateModel.Recipient,
            RecipientId = certificateModel.RecipientId,
            Stack = request.Model.Stack.GetDescription(),
            Technology = request.Model.Technology,
            UpdatedAt = certificateModel.UpdatedAt,
        };

        var userId = 555;
        var settings = await _settingsAccessorMock.Object.GetSettings(_cancellationToken);

        _identityContractMock.Setup(x => x.GetDepartmentModsenApiValue(certificateDto.Technology, _cancellationToken))
            .ReturnsAsync("JavaScript");

        _identityContractMock.Setup(x => x.UserRoles(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync([PortalRoleNames.Employee]);

        _mockCertificatesClient
            .Setup(x => x.CreateUserCertificate(
                It.IsAny<CreateCertificateModel>(),
                settings.MonthDayYearFormat,
                _cancellationToken))
            .ReturnsAsync(certificateModel);

        var teamBoardResponse = new Dictionary<int, bool>
        {
            [userId] = true
        };

        _teamBoardContract
            .Setup(x => x.CanUsersBeRetrievedByUser(new[] { userId }, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(teamBoardResponse);

        //Act
        var result = await _handler.Handle(request, _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(certificateDto);
    }

    [Fact]
    public async Task CreateCertificateWhenRequestIsInvalidThrowCertificateUnavailableException()
    {
        //Arrange
        var request = new CreateCertificateCommandUseCase.Command(new CreateCertificateDto
        {
            ReceiptDate = DateTime.UtcNow,
            Recipient = "Recipient",
            RecipientId = "555",
            Stack = null,
            Technology = "PMO"
        });

        var createCertificateModel = new CreateCertificateModel
        {
            ReceiptDate = request.Model.ReceiptDate,
            Recipient = request.Model.Recipient,
            RecipientId = request.Model.RecipientId,
            Stack = request.Model.Stack.ToString(),
            Technology = "PMO"
        };

        _identityContractMock.Setup(x => x.UserRoles(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync([PortalRoleNames.Employee]);

        _identityContractMock.Setup(x => x.GetDepartmentModsenApiValue(createCertificateModel.Technology, _cancellationToken))
            .ReturnsAsync("PMO");

        var settings = await _settingsAccessorMock.Object.GetSettings(_cancellationToken);
        var userId = 555;

        var teamBoardResponse = new Dictionary<int, bool>
        {
            [userId] = true
        };

        _teamBoardContract
            .Setup(x => x.CanUsersBeRetrievedByUser(new[] { userId }, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(teamBoardResponse);

        _mockCertificatesClient
            .Setup(x => x.CreateUserCertificate(
                It.IsAny<CreateCertificateModel>(),
                settings.MonthDayYearFormat,
                _cancellationToken))
            .ThrowsAsync(new CertificateUnavailableException());

        //Act
        var result = async () => await _handler.Handle(request, _cancellationToken);

        //Assert
        await result.Should().ThrowAsync<CertificateUnavailableException>();
    }
}
