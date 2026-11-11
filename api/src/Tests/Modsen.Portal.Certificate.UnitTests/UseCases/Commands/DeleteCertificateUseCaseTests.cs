using Modsen.Portal.Certificate.UseCases.Dto;
using Modsen.Portal.Certificate.UseCases.Requests;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Certificate;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.ModsenApi.Contract;
using Modsen.Portal.TeamBoard.Contract;
using Moq;

namespace Modsen.Portal.Certificate.UnitTests.UseCases.Commands;

public class DeleteCertificateUseCaseTests
{
    private readonly Mock<IModsenApiCertificatesClient> _apiCertificatesClientMock = new();
    private readonly Mock<ITeamBoardContract> _teamBoardContractMock = new();

    private readonly DeleteCertificateUseCase _useCase;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteCertificateUseCaseTests()
    {
        _useCase = new DeleteCertificateUseCase(_apiCertificatesClientMock.Object, _teamBoardContractMock.Object);
    }

    [Fact]
    public async Task HandleWhenProvidedUserIdIsNotANumberThrowInvalidDataAppException()
    {
        // Arrange
        var certificateId = Guid.CreateVersion7().ToAlphabetic();
        var userId = "userId";

        var command = new DeleteCertificateUseCase.Command(certificateId, userId);

        // Act
        var act = () => _useCase.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidDataAppException>();
    }

    [Fact]
    public async Task HandleWhenCurrentUserHasNoAccessThrowActionNotAllowed()
    {
        // Arrange
        var certificateId = Guid.CreateVersion7().ToAlphabetic();
        var userId = "123";

        var teamBoardResponse = new Dictionary<int, bool>
        {
            [123] = false
        };

        _teamBoardContractMock.Setup(x => x.CanUsersBeRetrievedByUser(new[] { 123 }, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(teamBoardResponse);

        var command = new DeleteCertificateUseCase.Command(certificateId, userId);

        // Act
        var act = () => _useCase.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenCurrentUserHasNoAccessEmptyDictThrowActionNotAllowed()
    {
        // Arrange
        var certificateId = Guid.CreateVersion7().ToAlphabetic();
        var userId = "123";

        var teamBoardResponse = new Dictionary<int, bool>();

        _teamBoardContractMock.Setup(x => x.CanUsersBeRetrievedByUser(new[] { 123 }, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(teamBoardResponse);

        var command = new DeleteCertificateUseCase.Command(certificateId, userId);

        // Act
        var act = () => _useCase.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenCertificateToDeleteIsNotIssuedToTheUserThrowActionNotAllowed()
    {
        // Arrange
        var certificateId = Guid.CreateVersion7().ToAlphabetic();
        var userId = "123";

        var teamBoardResponse = new Dictionary<int, bool>
        {
            [123] = true
        };

        _teamBoardContractMock.Setup(x => x.CanUsersBeRetrievedByUser(new[] { 123 }, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(teamBoardResponse);

        var userCertificates = new List<CertificateModel>
        {
            new CertificateModel
            {
                CertificateId = Guid.CreateVersion7().ToString(),
            },
            new CertificateModel
            {
                CertificateId = Guid.CreateVersion7().ToString(),
            }
        };

        _apiCertificatesClientMock.Setup(x => x.GetUserCertificates(userId, _cancellationToken))
            .ReturnsAsync(userCertificates);

        var command = new DeleteCertificateUseCase.Command(certificateId, userId);

        // Act
        var act = () => _useCase.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenRequestIsValidCallCertificatesApiClientAndReturnDeletedCertificate()
    {
        // Arrange
        var certificateId = Guid.CreateVersion7().ToAlphabetic();
        var userId = "123";

        var teamBoardResponse = new Dictionary<int, bool>
        {
            [123] = true
        };

        _teamBoardContractMock.Setup(x => x.CanUsersBeRetrievedByUser(new[] { 123 }, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(teamBoardResponse);

        var userCertificates = new List<CertificateModel>
        {
            new CertificateModel
            {
                CertificateId = certificateId
            },
            new CertificateModel
            {
                CertificateId = Guid.CreateVersion7().ToString(),
            }
        };

        _apiCertificatesClientMock.Setup(x => x.GetUserCertificates(userId, _cancellationToken))
            .ReturnsAsync(userCertificates);

        var certificateModel = new CertificateModel
        {
            CertificateId = certificateId,
            ReceiptDate = DateTime.UtcNow,
            Recipient = "Recipient",
            Stack = nameof(JsStackType.React),
            Technology = "Js",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var certificateDto = new CertificateDto
        {
            CertificateId = certificateModel.CertificateId,
            CreatedAt = certificateModel.CreatedAt,
            ReceiptDate = certificateModel.ReceiptDate,
            Recipient = certificateModel.Recipient,
            RecipientId = certificateModel.RecipientId,
            Stack = JsStackType.React.GetDescription(),
            Technology = certificateModel.Technology,
            UpdatedAt = certificateModel.UpdatedAt,
        };

        _apiCertificatesClientMock
            .Setup(x => x.DeleteUserCertificate(certificateId, _cancellationToken))
            .ReturnsAsync(certificateModel);

        var command = new DeleteCertificateUseCase.Command(certificateId, userId);

        // Act
        var result = await _useCase.Handle(command, _cancellationToken);

        //Assert
        _apiCertificatesClientMock.Verify(x =>
            x.DeleteUserCertificate(certificateId, _cancellationToken), Times.Once);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(certificateDto);
    }
}
