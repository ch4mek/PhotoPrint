using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.Requests.Reports.Requests.RequestDtos;

namespace Modsen.Portal.Equipment.Requests.Reports.Requests;

internal sealed class GetEquipmentReportByIdRequest : IQueryHandler<GetEquipmentReportByIdRequest.Query, EquipmentReportDto>
{
    private readonly IEquipmentDbContext _equipmentDbContext;

    public GetEquipmentReportByIdRequest(IEquipmentDbContext equipmentDbContext)
    {
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task<EquipmentReportDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var equipmentReport = await _equipmentDbContext.EquipmentReports
            .AsNoTracking()
            .Where(equipmentReport => equipmentReport.Id == request.Id)
            .Select(equipmentReport => new EquipmentReportDto
            {
                Id = equipmentReport.Id,
                SerialNumber = equipmentReport.SerialNumber,
                Data = equipmentReport.Data,
                CreatedAtUtc = equipmentReport.CreatedAtUtc,
                AssembledAtUtc = equipmentReport.AssembledAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (equipmentReport is null)
        {
            throw new EntityNotFoundException(PortalMessages.EquipmentReportNotFound);
        }

        return equipmentReport;
    }

    internal sealed record Query(int Id) : IQuery<EquipmentReportDto>, IEquipmentRequest;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(query => query.Id)
                .GreaterThan(0);
        }
    }
}
