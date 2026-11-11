using System.Globalization;
using System.Net.Mail;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Storage.Contract;
using Modsen.Portal.Storage.Contract.Models;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Payslips.Core.Encryptor;
using Modsen.Portal.TeamBoard.Requests.Payslips.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Payslips.Requests;

internal sealed class UploadPayslipRequest: ICommandHandler<UploadPayslipRequest.Command, PayslipDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IClockService _clockService;
    private readonly IPayslipEncryptor _payslipEncryptor;
    private readonly IConfiguration _configuration;
    private readonly IStorageCrossWriterContract _storageContract;

    public UploadPayslipRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IStorageCrossWriterContract storageContract,
        IClockService clockService,
        IPayslipEncryptor payslipEncryptor,
        IConfiguration configuration)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _storageContract = storageContract;
        _clockService = clockService;
        _payslipEncryptor = payslipEncryptor;
        _configuration = configuration;
    }

    public async Task<PayslipDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var emailToSearch = GetUserEmail(request.UserEmail);
        var user = await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.Email.ToLower() == emailToSearch.ToLower())
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        if (user.Position.IsIntern())
        {
            throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
        }

        var period = GetPeriodAsDate(request.Period, user.HireDate);
        var monthNameEng = period.ToString("MMMM", new CultureInfo("en-US"));
        var fileName = $"Payslip_{user.LastName}_{user.FirstName}_{monthNameEng}.pdf";
        var fileId = await SaveFileToStorage(request, fileName, cancellationToken);

        var payslip = new Payslip
        {
            FileId = fileId,
            FileName = fileName,
            ReportDate = period,
            UserId = user.UserId,
            CreatedAtUtc = _clockService.UtcNow
        };

        await _teamBoardDbContext.Payslips.AddAsync(payslip, cancellationToken);
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return new PayslipDto
        {
            FileId = payslip.FileId,
            FileName = fileName,
            ReportDate = payslip.ReportDate,
            UserId = user.UserId,
            CreatedAtUtc = payslip.CreatedAtUtc,
        };
    }

    private async Task<string> SaveFileToStorage(Command request, string fileName, CancellationToken cancellationToken)
    {
        const string contentType = "application/pdf";
        var fileContent = Convert.FromBase64String(request.FileContent);

        var encryptedFile = _payslipEncryptor.Encrypt(fileContent);

        if (encryptedFile is null)
        {
            throw new InvalidDataAppException("Some error occured while encrypting the file");
        }

        try
        {
            return await _storageContract.UploadFileInternal(new UploadFileDto
            {
                FileContent = encryptedFile,
                FileName = fileName,
                FileSize = encryptedFile.LongLength,
                UserId = null,
                AvailableTill = null,
                ContentType = contentType,
                FileSource = FileSource.System,           
            }, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidDataAppException($"Error occured while saving the file: {ex.Message}");
        }
    }

    private string GetUserEmail(string oldEmail)
    {
        var domain = _configuration.GetValue<string>("AppSettings:AuthSettings:Domain")
            ?? "@modsen-software.com";

        var mail = new MailAddress(oldEmail);
        var prefix = mail.User;

        return $"{prefix}{domain}";
    }

    private static DateOnly GetPeriodAsDate(string period, DateOnly? hireDate)
    {
        period = period.Replace("г.", "", StringComparison.InvariantCulture).Trim();
        DateTime date;

        try
        {
            date = DateTime.ParseExact(period, "MMMM yyyy", new CultureInfo("ru-RU"));
        }
        catch (FormatException)
        {
            throw new ValidationException("Invalid Period format");
        }

        var day = hireDate?.Day ?? 1;

        var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

        var finalDay = Math.Min(day, daysInMonth);

        var resultPeriod = new DateTime(date.Year, date.Month, finalDay, 0, 0, 0, DateTimeKind.Utc);

        return DateOnly.FromDateTime(resultPeriod);
    }

    internal sealed record Command(string FileContent, string Period, string UserEmail)
        : ICommand<PayslipDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FileContent)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Period)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.UserEmail)
                .NotNull()
                .EmailAddress();
        }
    }
}
