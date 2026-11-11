using FluentValidation;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Storage.Contract;
using Modsen.Portal.Storage.Contract.Models;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Payslips.Core.Encryptor;
using Modsen.Portal.TeamBoard.Requests.Payslips.Requests;
using Modsen.Portal.TeamBoard.Requests.Payslips.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Payslips;

public class UploadPayslipRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<IPayslipEncryptor> _payslipEncryptorMock = new();
    private readonly Mock<IStorageCrossWriterContract> _storageContractMock = new();

    private readonly CancellationToken _token = CancellationToken.None;
    private readonly UploadPayslipRequest _handler;

    public UploadPayslipRequestTests()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"AppSettings:AuthSettings:Domain", "@modsen-software.com"},
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _handler = new UploadPayslipRequest(
            _dbContextMock.Object,
            _storageContractMock.Object,
            _clockServiceMock.Object,
            _payslipEncryptorMock.Object,
            configuration);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var command = new UploadPayslipRequest.Command(
            FileContent: "YWxhbGFsYWxsYWxhYQ==",
            Period: "Июнь 2025 г.",
            UserEmail: "nonexistent@modsen.by");

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>().BuildMockDbSet().Object);

        // Act
        var action = () => _handler.Handle(command, _token);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenFileEncryptionFailsShouldThrowInvalidDataAppException()
    {
        // Arrange
        var userEmail = "test@modsen-software.com";

        var command = new UploadPayslipRequest.Command(
            FileContent: "YWxhbGFsYWxsYWxhYQ==",
            Period: "Июнь 2025 г.",
            UserEmail: "test@modsen.by");

        var user = new TeamBoardUser
        {
            UserId = 1,
            Email = userEmail,
            FirstName = "test",
            LastName = "test",
        };

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user }.BuildMockDbSet().Object);

        _payslipEncryptorMock.Setup(x => x.Encrypt(It.IsAny<byte[]>()))
            .Returns((byte[])null!);

        // Act
        var action = () => _handler.Handle(command, _token);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage("Some error occured while encrypting the file");
    }

    [Fact]
    public async Task HandleWhenUserIsInternShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userEmail = "intern@modsen-software.com";

        var command = new UploadPayslipRequest.Command(
            FileContent: "YWxhbGFsYWxsYWxhYQ==",
            Period: "Июнь 2025 г.",
            UserEmail: "intern@modsen.by");

        var user = new TeamBoardUser
        {
            UserId = 1,
            Email = userEmail,
            FirstName = "test",
            LastName = "test",
            Position = PortalRoleNames.Intern
        };

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user }.BuildMockDbSet().Object);

        // Act
        var action = () => _handler.Handle(command, _token);

        // Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.NotAllowedOperationWithIntern);
    }

    [Fact]
    public async Task HandleWhenStorageUploadFailsShouldThrowInvalidDataAppException()
    {
        // Arrange
        var userEmail = "testtest@modsen-software.com";

        var command = new UploadPayslipRequest.Command(
            FileContent: "YWxhbGFsYWxsYWxhYQ==",
            Period: "Июнь 2025 г.",
            UserEmail: "testtest@modsen.by");

        var user = new TeamBoardUser
        {
            UserId = 1,
            Email = userEmail,
            FirstName = "test",
            LastName = "test",
        };

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user }.BuildMockDbSet().Object);

        _payslipEncryptorMock.Setup(x => x.Encrypt(It.IsAny<byte[]>()))
            .Returns([4, 5, 6]);

        _storageContractMock.Setup(x => x.UploadFileInternal(
                It.IsAny<UploadFileDto>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Storage error"));

        // Act
        var action = () => _handler.Handle(command, _token);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage("Error occured while saving the file: Storage error");
    }

    [Fact]
    public async Task ValidatorWhenDateIsIncorrectShouldThrowValidationException()
    {
        // Arrange
        var userEmail = "TestTest@modsen-software.com";

        var command = new UploadPayslipRequest.Command(
            FileContent: "YWxhbGFsYWxsYWxhYQ==",
            Period: "Июньmm 2025 г.",
            UserEmail: "testtest@modsen.by");

        var user = new TeamBoardUser
        {
            UserId = 1,
            Email = userEmail,
            FirstName = "test",
            LastName = "test",
        };

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user }.BuildMockDbSet().Object);

        _payslipEncryptorMock.Setup(x => x.Encrypt(It.IsAny<byte[]>()))
            .Returns([4, 5, 6]);

        // Act
        var action = () => _handler.Handle(command, _token);

        // Assert
        await action.Should()
            .ThrowAsync<ValidationException>()
            .WithMessage("Invalid Period format");
    }

    [Fact]
    public async Task HandleWhenUserHasHireDateAndMonthDoesNotHaveLastDayShouldCreatePayslipAndReturnPayslipDtoWithHireDateDay()
    {
        // Arrange
        var userEmail = "testTest@modsen-software.com";
        var fileContent = "YWxhbGFsYWxsYWxhYQ==";
        var encryptedFileContent = new byte[] { 4, 5, 6 };
        var fileId = "testtest";
        var createdAtUtc = DateTime.UtcNow;

        var command = new UploadPayslipRequest.Command(
            FileContent: fileContent,
            Period: "Февраль 2025 г.",
            UserEmail: "testtest@modsen.by");

        var user = new TeamBoardUser
        {
            UserId = 1,
            Email = userEmail,
            FirstName = "first",
            LastName = "last",
            HireDate = new DateOnly(2020, 1, 31)
        };

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user }.BuildMockDbSet().Object);

        _payslipEncryptorMock.Setup(x => x.Encrypt(It.IsAny<byte[]>()))
            .Returns(encryptedFileContent);

        _storageContractMock.Setup(x => x.UploadFileInternal(
                It.IsAny<UploadFileDto>(),
                _token))
            .ReturnsAsync(fileId);

        _dbContextMock.Setup(x => x.Payslips.AddAsync(It.IsAny<Payslip>(), _token))
            .Callback<Payslip, CancellationToken>((payslip, _) =>
            {
                payslip.FileId = fileId;
                payslip.FileName = "Payslip_last_first_February.pdf";
                payslip.ReportDate = new DateOnly(2025, 2, 28);
                payslip.UserId = user.UserId;
                payslip.CreatedAtUtc = createdAtUtc;
            });

        _dbContextMock.Setup(x => x.SaveChangesAsync(_token))
            .ReturnsAsync(1);

        _clockServiceMock.Setup(x => x.UtcNow)
            .Returns(createdAtUtc);

        var expectedDto = new PayslipDto
        {
            FileId = fileId,
            FileName = "Payslip_last_first_February.pdf",
            ReportDate = new DateOnly(2025, 2, 28),
            UserId = user.UserId,
            CreatedAtUtc = createdAtUtc
        };

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _dbContextMock.Verify(x => x.Payslips.AddAsync(It.Is<Payslip>(p =>
            p.FileId == fileId &&
            p.FileName == "Payslip_last_first_February.pdf" &&
            p.ReportDate == new DateOnly(2025, 2, 28) &&
            p.UserId == user.UserId &&
            p.CreatedAtUtc == createdAtUtc), _token), Times.Once());
        _dbContextMock.Verify(x => x.SaveChangesAsync(_token), Times.Once());
    }

    [Fact]
    public async Task HandleWhenUserHasHireDateShouldCreatePayslipAndReturnPayslipDtoWithHireDateDay()
    {
        // Arrange
        var userEmail = "testTest@modsen-software.com";
        var fileContent = "YWxhbGFsYWxsYWxhYQ==";
        var encryptedFileContent = new byte[] { 4, 5, 6 };
        var fileId = "testtest";
        var createdAtUtc = DateTime.UtcNow;

        var command = new UploadPayslipRequest.Command(
            FileContent: fileContent,
            Period: "Июнь 2025 г.",
            UserEmail: "testtest@modsen.by");

        var user = new TeamBoardUser
        {
            UserId = 1,
            Email = userEmail,
            FirstName = "first",
            LastName = "last",
            HireDate = new DateOnly(2020, 1, 16)
        };

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user }.BuildMockDbSet().Object);

        _payslipEncryptorMock.Setup(x => x.Encrypt(It.IsAny<byte[]>()))
            .Returns(encryptedFileContent);

        _storageContractMock.Setup(x => x.UploadFileInternal(
                It.IsAny<UploadFileDto>(),
                _token))
            .ReturnsAsync(fileId);

        _dbContextMock.Setup(x => x.Payslips.AddAsync(It.IsAny<Payslip>(), _token))
            .Callback<Payslip, CancellationToken>((payslip, _) =>
            {
                payslip.FileId = fileId;
                payslip.FileName = "Payslip_last_first_June.pdf";
                payslip.ReportDate = new DateOnly(2025, 6, 16);
                payslip.UserId = user.UserId;
                payslip.CreatedAtUtc = createdAtUtc;
            });

        _dbContextMock.Setup(x => x.SaveChangesAsync(_token))
            .ReturnsAsync(1);

        _clockServiceMock.Setup(x => x.UtcNow)
            .Returns(createdAtUtc);

        var expectedDto = new PayslipDto
        {
            FileId = fileId,
            FileName = "Payslip_last_first_June.pdf",
            ReportDate = new DateOnly(2025, 6, 16),
            UserId = user.UserId,
            CreatedAtUtc = createdAtUtc
        };

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _dbContextMock.Verify(x => x.Payslips.AddAsync(It.Is<Payslip>(p =>
            p.FileId == fileId &&
            p.FileName == "Payslip_last_first_June.pdf" &&
            p.ReportDate == new DateOnly(2025, 6, 16) &&
            p.UserId == user.UserId &&
            p.CreatedAtUtc == createdAtUtc), _token), Times.Once());
        _dbContextMock.Verify(x => x.SaveChangesAsync(_token), Times.Once());
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldCreatePayslipAndReturnPayslipDto()
    {
        // Arrange
        var userEmail = "testtest@modsen-software.com";
        var fileContent = "YWxhbGFsYWxsYWxhYQ==";
        var encryptedFileContent = new byte[] { 4, 5, 6 };
        var fileId = "testtest";
        var createdAtUtc = DateTime.UtcNow;

        var command = new UploadPayslipRequest.Command(
            FileContent: fileContent,
            Period: "Июнь 2025 г.",
            UserEmail: "testtest@modsen.by");

        var user = new TeamBoardUser
        {
            UserId = 1,
            FirstName = "first",
            LastName = "last",
            Email = userEmail
        };

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user }.BuildMockDbSet().Object);

        _payslipEncryptorMock.Setup(x => x.Encrypt(It.IsAny<byte[]>()))
            .Returns(encryptedFileContent);

        _storageContractMock.Setup(x => x.UploadFileInternal(
                It.IsAny<UploadFileDto>(),
                _token))
            .ReturnsAsync(fileId);

        _dbContextMock.Setup(x => x.Payslips.AddAsync(It.IsAny<Payslip>(), _token))
            .Callback<Payslip, CancellationToken>((payslip, _) =>
            {
                payslip.FileId = fileId;
                payslip.FileName = "Payslip_last_first_June.pdf";
                payslip.ReportDate = new DateOnly(2025, 6, 1);
                payslip.UserId = user.UserId;
                payslip.CreatedAtUtc = createdAtUtc;
            });

        _dbContextMock.Setup(x => x.SaveChangesAsync(_token))
            .ReturnsAsync(1);

        _clockServiceMock.Setup(x => x.UtcNow)
            .Returns(createdAtUtc);

        var expectedDto = new PayslipDto
        {
            FileId = fileId,
            FileName = "Payslip_last_first_June.pdf",
            ReportDate = new DateOnly(2025, 6, 1),
            UserId = user.UserId,
            CreatedAtUtc = createdAtUtc
        };

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _dbContextMock.Verify(x => x.Payslips.AddAsync(It.Is<Payslip>(p =>
            p.FileId == fileId &&
            p.FileName == "Payslip_last_first_June.pdf" &&
            p.ReportDate == new DateOnly(2025, 6, 1) &&
            p.UserId == user.UserId &&
            p.CreatedAtUtc == createdAtUtc), _token), Times.Once());
        _dbContextMock.Verify(x => x.SaveChangesAsync(_token), Times.Once());
    }
}
