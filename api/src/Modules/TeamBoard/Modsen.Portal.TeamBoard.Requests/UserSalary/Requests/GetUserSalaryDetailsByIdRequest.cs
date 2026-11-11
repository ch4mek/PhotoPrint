using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Requests;

internal sealed class GetUserSalaryDetailsByIdRequest
    : IQueryHandler<GetUserSalaryDetailsByIdRequest.Query, SalaryDetailsDto>
{
    private readonly ICanUserAccessSalaryBusinessRule _canUserAccessSalaryBusinessRule;
    private readonly IUserSalaryLoader _salaryLoader;

    public GetUserSalaryDetailsByIdRequest(
        IUserSalaryLoader salaryLoader,
        ICanUserAccessSalaryBusinessRule canUserAccessSalaryBusinessRule)
    {
        _salaryLoader = salaryLoader;
        _canUserAccessSalaryBusinessRule = canUserAccessSalaryBusinessRule;
    }

    public async Task<SalaryDetailsDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var userSalary = await _salaryLoader.LoadAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserSalaryNotFound);

        var hasAccess = await _canUserAccessSalaryBusinessRule.IsCorrespondsToBusiness(userSalary.UserId,
            cancellationToken);
        if (!hasAccess)
        {
            throw new ActionNotAllowedException();
        }

        return new SalaryDetailsDto(userSalary.Id, userSalary.UserId, userSalary.SalaryFrom, userSalary.SalaryUsd);
    }

    internal sealed record Query(int Id) : IQuery<SalaryDetailsDto>;
}
