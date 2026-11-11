using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.Requests.Reports.Requests.RequestDtos;

namespace Modsen.Portal.Equipment.Requests.Reports.Requests;

internal sealed class GetEquipmentReportRelevancePeriodsRequest : IQueryHandler<GetEquipmentReportRelevancePeriodsRequest.Query, List<EquipmentReportRelevancePeriodDto>>
{
    private readonly IEquipmentDbContext _equipmentDbContext;

    public GetEquipmentReportRelevancePeriodsRequest(IEquipmentDbContext equipmentDbContext)
    {
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task<List<EquipmentReportRelevancePeriodDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var equipmentReportRelevancePeriods = await _equipmentDbContext.EquipmentReportRelevancePeriods
            .AsNoTracking()
            .Where(equipmentReport => EF.Functions.Like(equipmentReport.EquipmentReport!.SerialNumber, request.SerialNumber))
            .Select(equipmentReport => new EquipmentReportRelevancePeriodDto
            {
                FromUtc = equipmentReport.FromUtc,
                ToUtc = equipmentReport.ToUtc,
                EquipmentReportId = equipmentReport.EquipmentReportId
            })
            .OrderByDescending(equipmentReportRelevancePeriod => equipmentReportRelevancePeriod.ToUtc)
            .ToListAsync(cancellationToken);

        if (equipmentReportRelevancePeriods.Count == 0)
        {
            throw new EntityNotFoundException(PortalMessages.EquipmentReportRelevancePeriodsNotFound);
        }

        return equipmentReportRelevancePeriods;
    }

    internal sealed record Query(string SerialNumber) : IQuery<List<EquipmentReportRelevancePeriodDto>>, IEquipmentRequest;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(query => query.SerialNumber)
                .NotEmpty();
        }
    }
}
