using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;
using Modsen.Portal.Equipment.Requests.Users.BusinessRules;
using Modsen.Portal.Equipment.Requests.Users.Requests.RequestDtos;

namespace Modsen.Portal.Equipment.Requests.Users.Requests;

internal sealed class GetUserEquipmentRequest : IQueryHandler<GetUserEquipmentRequest.Query, PageableUserEquipmentDto>
{
    private readonly IEquipmentDbContext _equipmentDbContext;
    private readonly IIsAllowedToGetUserEquipmentBusinessRule _isAllowedToGetUserEquipmentBusinessRule;

    public GetUserEquipmentRequest(
        IEquipmentDbContext equipmentDbContext,
        IIsAllowedToGetUserEquipmentBusinessRule isAllowedToGetUserEquipmentBusinessRule)
    {
        _equipmentDbContext = equipmentDbContext;
        _isAllowedToGetUserEquipmentBusinessRule = isAllowedToGetUserEquipmentBusinessRule;
    }

    public async Task<PageableUserEquipmentDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var isUserExist = await _equipmentDbContext.EquipmentUsers
            .AnyAsync(entity => entity.UserId == request.UserId, cancellationToken);

        if (!isUserExist)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        if (!await _isAllowedToGetUserEquipmentBusinessRule.IsCorrespondsToBusiness(request.UserId, cancellationToken))
        {
            throw new ActionNotAllowedException(PortalMessages.YouDoNotHavePermission);
        }

        var equipments = await GetPageableEquipment(request.UserId, request.QueryOptions, cancellationToken);

        return new PageableUserEquipmentDto
        {
            Items = equipments.Items,
            Page = equipments.Page,
            PageSize = equipments.PageSize,
            Total = equipments.Total
        };
    }

    private async Task<PageableListOfItems<OdpUserAssignments>> GetPageableEquipment(
        int userId,
        ODataQueryOptions<OdpUserAssignments> queryOptions,
        CancellationToken cancellationToken)
    {
        var query = _equipmentDbContext.EquipmentAssigns
            .Where(x => x.AssignedToUserId == userId)
            .Select(OdpUserAssignments.Map)
            .AsQueryable();

        var queryOptionsWithoutPagination = queryOptions.RebuildWithoutPagination();

        var result = await queryOptions
            .ApplyTo(query)
            .Cast<OdpUserAssignments>()
            .ToListAsync(cancellationToken);

        var totalCount = await queryOptionsWithoutPagination
            .ApplyTo(query)
            .OfType<OdpUserAssignments>()
            .CountAsync(cancellationToken);

        return result.ToPageable(queryOptions, totalCount);
    }

    internal sealed record Query(int UserId, ODataQueryOptions<OdpUserAssignments> QueryOptions)
        : IQuery<PageableUserEquipmentDto>, IEquipmentRequest;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
