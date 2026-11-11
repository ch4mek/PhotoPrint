using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;
using Modsen.Portal.Equipment.Requests.Core.BusinessRules;
using Modsen.Portal.Equipment.Requests.Equipment.Core.CoreDto;
using Modsen.Portal.Equipment.Requests.Equipment.Core.Exporters;
using Modsen.Portal.Equipment.Requests.EquipmentPortalSettings;

namespace Modsen.Portal.Equipment.Requests.Equipment.Requests;

internal sealed class GetEquipmentsByFileRequest : IQueryHandler<GetEquipmentsByFileRequest.Query, byte[]>
{
    private readonly ICanAccessUnassignedEquipmentBusinessRule _canAccessUnassignedEquipmentBusinessRule;
    private readonly IEquipmentPortalSettingsAccessor _equipmentPortalSettingsAccessor;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IEquipmentToExcelExporter _excelExporter;
    private readonly IEquipmentDbContext _equipmentDbContext;

    public GetEquipmentsByFileRequest(
        ICanAccessUnassignedEquipmentBusinessRule canAccessUnassignedEquipmentBusinessRule,
        IEquipmentPortalSettingsAccessor equipmentPortalSettingsAccessor,
        IAuthenticatedUserContext authenticatedUserContext,
        IEquipmentToExcelExporter excelExporter,
        IEquipmentDbContext equipmentDbContext)
    {
        _canAccessUnassignedEquipmentBusinessRule = canAccessUnassignedEquipmentBusinessRule;
        _equipmentPortalSettingsAccessor = equipmentPortalSettingsAccessor;
        _authenticatedUserContext = authenticatedUserContext;
        _equipmentDbContext = equipmentDbContext;
        _excelExporter = excelExporter;
    }

    public async Task<byte[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var hasManageAllClaim = _authenticatedUserContext.HasClaim(EquipmentClaimTypeNames.ManageAllEquipment.Name);
        var equipments = await GetEquipmentAsync(request, hasManageAllClaim, cancellationToken);

        var activeHoldersDepartmentIds = equipments
            .Where(equipment => equipment.ActiveHolderDepartmentId is not null)
            .Select(equipment => equipment.ActiveHolderDepartmentId)
            .Distinct()
            .ToList();

        var departmentIds = activeHoldersDepartmentIds.ToArray();
        var equipmentDepartments = await GetEquipmentDepartmentsAsync(departmentIds!, cancellationToken);

        var shouldPrintPurchaseDetails = _authenticatedUserContext.GetRoles().IsSystemAdministrator() ||
            _authenticatedUserContext.GetRoles().IsCto() ||
            _authenticatedUserContext.GetRoles().IsCeo() ||
            hasManageAllClaim;

        var shouldPrintDepartment = !(_authenticatedUserContext.GetRoles().IsHeadOfDepartment() ||
            _authenticatedUserContext.GetRoles().IsGroupManager()) || hasManageAllClaim;

        var settings = await _equipmentPortalSettingsAccessor.GetSettings(cancellationToken);

        var equipmentToExcelExportModels = equipments.ConvertAll(equipment =>
            EquipmentToExcelExportCoreDto.Of(equipment, equipmentDepartments));

        ArgumentException.ThrowIfNullOrEmpty(settings.DefaultDateFormat);

        return request.Format switch
        {
            DocumentType.Excel =>
                await _excelExporter.Export(new EquipmentsToExcelExportCoreDto
                {
                    Equipments = equipmentToExcelExportModels,
                    ShouldPrintPurchaseDetails = shouldPrintPurchaseDetails,
                    SummaryReportWorksheet = settings.SummaryReportWorksheet,
                    BaseFontSize = settings.BaseFontSize,
                    DefaultColumnWidthInSymbols = settings.DefaultColumnWidthInSymbols,
                    DefaultDateFormat = settings.DefaultDateFormat,
                    ShouldPrintDepartment = shouldPrintDepartment,
                }, cancellationToken),

            _ => throw new InvalidDataAppException(PortalMessages.UnsupportedFileFormat)
        };
    }

    private async Task<List<OdpEquipment>> GetEquipmentAsync(Query request, bool hasManageAllClaim, CancellationToken cancellationToken)
    {
        var isAvailableEquipmentsWithoutAssigners = _canAccessUnassignedEquipmentBusinessRule
            .IsCorrespondsToBusiness(_authenticatedUserContext);

        var departmentId = _authenticatedUserContext.DepartmentId;
        var resourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager()
            ? _authenticatedUserContext.ResourceGroupId
            : null;

        var query = _equipmentDbContext.Equipments
            .Select(OdpEquipment.Map)
            .AsQueryable();

        if (!isAvailableEquipmentsWithoutAssigners && !hasManageAllClaim)
        {
            query = query.Where(x => x.ActiveHolderId != null);

            if (departmentId is not null)
            {
                query = query.Where(x => x.ActiveHolderDepartmentId == departmentId);
            }

            if (resourceGroupId is not null)
            {
                query = query.Where(x => x.ActiveHolderResourceGroupId == resourceGroupId);
            }
        }

        return await request.QueryOptions
            .ApplyTo(query)
            .Cast<OdpEquipment>()
            .ToListAsync(cancellationToken);
    }

    private Task<List<EquipmentDepartmentCoreDto>> GetEquipmentDepartmentsAsync(string[] departmentIds,
        CancellationToken cancellationToken)

    {
        return _equipmentDbContext.EquipmentDepartments
            .Where(department => departmentIds.Contains(department.DepartmentId))
            .Select(department => new EquipmentDepartmentCoreDto
            {
                Id = department.DepartmentId,
                FullName = department.FullName,
                ShortName = department.ShortName
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(ODataQueryOptions<OdpEquipment> QueryOptions, DocumentType Format) : IQuery<byte[]>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Format).IsInEnum();

            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
