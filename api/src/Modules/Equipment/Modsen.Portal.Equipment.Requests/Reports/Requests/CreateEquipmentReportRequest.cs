using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Reports.Requests.RequestDtos;

namespace Modsen.Portal.Equipment.Requests.Reports.Requests;
internal sealed class CreateEquipmentReportRequest : ICommandHandler<CreateEquipmentReportRequest.Command, EquipmentReportDto>
{
    private const string s_assemblyDatePropertyName = "CreationDate";
    private const string s_serialNumberPropertyName = "HardwareSerialNumber";
    private const string s_appTracingInfo = "AppTracingInfo";

    private readonly IEquipmentDbContext _equipmentDbContext;
    private readonly IClockService _clockService;
    private readonly IHashService _hashService;

    public CreateEquipmentReportRequest(
        IEquipmentDbContext equipmentDbContext,
        IClockService clockService,
        IHashService hashService)
    {
        _equipmentDbContext = equipmentDbContext;
        _clockService = clockService;
        _hashService = hashService;
    }

    public async Task<EquipmentReportDto> Handle(Command request, CancellationToken cancellationToken)
    {
        await using var stream = request.File.OpenReadStream();

        var jsonDocument = await JsonDocument.ParseAsync(stream,
            cancellationToken: cancellationToken);

        var jsonRootElement = jsonDocument.RootElement;

        var jsonObject = JsonObject.Create(jsonRootElement);

        if (jsonObject is null)
        {
            throw new InvalidDataAppException();
        }

        var serialNumber = await GetSerialNumberFromJsonObjectAsync(jsonObject, cancellationToken);

        var creationDate = GetCreationDateTokenFromJsonObject(jsonObject);
        var jsonData = jsonObject.ToJsonString( new JsonSerializerOptions { WriteIndented = false });
        var jsonDataHash = GetPreparedJsonDataHash(jsonObject);

        var equipmentReportDto = await _equipmentDbContext.EquipmentReports
            .AsNoTracking()
            .Where(equipmentReport => EF.Functions.Like(equipmentReport.SerialNumber, serialNumber) && equipmentReport.DataHash == jsonDataHash)
            .Select(equipmentReport => new EquipmentReportDto
            {
                Id = equipmentReport.Id,
                SerialNumber = equipmentReport.SerialNumber,
                Data = equipmentReport.Data,
                CreatedAtUtc = equipmentReport.CreatedAtUtc,
                AssembledAtUtc = equipmentReport.AssembledAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (equipmentReportDto is null)
        {
            var utcCreationDateTime = creationDate.ToUniversalTime();

            equipmentReportDto = await CreateEquipmentReportAsync(
                serialNumber,
                jsonData,
                jsonDataHash,
                utcCreationDateTime,
                cancellationToken);
        }
        else
        {
            await UpdateEquipmentPeriodAsync(equipmentReportDto.SerialNumber, equipmentReportDto.Id, cancellationToken);
        }

        await _equipmentDbContext.SaveChangesAsync(cancellationToken);

        return equipmentReportDto;
    }

    private async Task UpdateEquipmentPeriodAsync(string serialNumber, int equipmentReportId, CancellationToken cancellationToken)
    {
        var relevantReportPeriod = await _equipmentDbContext.EquipmentReportRelevancePeriods
                .Include(reportRelevancePeriod => reportRelevancePeriod.EquipmentReport)
                .OrderByDescending(reportRelevancePeriod => reportRelevancePeriod.ToUtc)
                .Where(reportRelevancePeriod => reportRelevancePeriod.EquipmentReport!.SerialNumber == serialNumber)
                .FirstOrDefaultAsync(cancellationToken);

        if (relevantReportPeriod?.EquipmentReportId == equipmentReportId)
        {
            relevantReportPeriod.ToUtc = _clockService.UtcNow;
        }
        else if (relevantReportPeriod is not null)
        {
            var period = new EquipmentReportRelevancePeriod
            {
                EquipmentReportId = equipmentReportId,
                ToUtc = _clockService.UtcNow,
                FromUtc = _clockService.UtcNow
            };

            await _equipmentDbContext.EquipmentReportRelevancePeriods.AddAsync(period, cancellationToken);
        }
    }

    private async Task<EquipmentReportDto> CreateEquipmentReportAsync(
        string serialNumber,
        string jsonData,
        string jsonDataHash,
        DateTime utcCreationDateTime,
        CancellationToken cancellationToken)
    {
        var equipmentReportEntity = new EquipmentReport
        {
            SerialNumber = serialNumber,
            Data = jsonData,
            DataHash = jsonDataHash,
            AssembledAtUtc = utcCreationDateTime,
        };

        var equipmentReportRelevancePeriod = new EquipmentReportRelevancePeriod
        {
            FromUtc = _clockService.UtcNow,
            ToUtc = _clockService.UtcNow
        };

        equipmentReportEntity.EquipmentReportRelevancePeriods.Add(equipmentReportRelevancePeriod);

        await _equipmentDbContext.EquipmentReports.AddAsync(equipmentReportEntity, cancellationToken);

        return new EquipmentReportDto
        {
            SerialNumber = equipmentReportEntity.SerialNumber,
            Data = equipmentReportEntity.Data,
            AssembledAtUtc = equipmentReportEntity.AssembledAtUtc,
        };
    }

    private async Task<string> GetSerialNumberFromJsonObjectAsync(
        JsonObject jsonObject,
        CancellationToken cancellationToken)
    {
        jsonObject.TryGetPropertyValue(s_serialNumberPropertyName, out var serialNumberProperty);

        var serialNumber = serialNumberProperty?.GetValue<string>();
        jsonObject.Remove(s_serialNumberPropertyName);

        if (serialNumber is null)
        {
            throw new InvalidDataAppException(
                PortalMessages.EquipmentReportSerialNumberNotFound);
        }

        var isEquipmentExisting = await _equipmentDbContext.Equipments
            .AnyAsync(equipment => equipment.SerialNumber == serialNumber, cancellationToken);

        if (!isEquipmentExisting)
        {
            throw new InvalidDataAppException(
                PortalMessages.EquipmentNotFound);
        }

        return serialNumber.ToLower(CultureInfo.InvariantCulture);
    }

    private static DateTime GetCreationDateTokenFromJsonObject(
        JsonObject jsonObject)
    {
        jsonObject.TryGetPropertyValue(s_assemblyDatePropertyName, out var creationDateProperty);

        var creationDate = creationDateProperty?.GetValue<DateTime>();

        if (creationDate is null)
        {
            throw new InvalidDataAppException(
                PortalMessages.EquipmentReportAssemblyDateNotFound);
        }

        jsonObject.Remove(s_assemblyDatePropertyName);

        return (DateTime)creationDate;
    }

    private string GetPreparedJsonDataHash(
        JsonObject jsonObject)
    {
        jsonObject.Remove(s_appTracingInfo);

        return _hashService.GetSha256Hash(jsonObject.ToJsonString(
                            new JsonSerializerOptions { WriteIndented = false }));
    }

    internal sealed record Command(
        IFormFile File) : ICommand<EquipmentReportDto>, IEquipmentRequest, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.File)
                .Must(file => file.FileName.EndsWith("json", StringComparison.InvariantCulture))
                .WithMessage("File extension is not json");
        }
    }
}
