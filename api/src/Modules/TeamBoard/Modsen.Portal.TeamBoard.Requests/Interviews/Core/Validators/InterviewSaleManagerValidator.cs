using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;

namespace Modsen.Portal.TeamBoard.Requests.Interviews.Core.Validators;

internal interface IInterviewSaleManagerValidator
{
    Task Validate(InterviewSaleManagerValidator.Request? model, CancellationToken cancellationToken, bool allowCeoCto = false);
}

internal class InterviewSaleManagerValidator : IInterviewSaleManagerValidator
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardPortalSettingsAccessor _teamBoardPortalSettingsAccessor;

    public InterviewSaleManagerValidator(IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardPortalSettingsAccessor teamBoardPortalSettingsAccessor)
    {
        _teamBoardPortalSettingsAccessor = teamBoardPortalSettingsAccessor;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task Validate(Request? model, CancellationToken cancellationToken, bool allowCeoCto = false)
    {
        ArgumentNullException.ThrowIfNull(model);

        var settings = await _teamBoardPortalSettingsAccessor.GetSettings(cancellationToken);

        var salesDepartmentId = settings.SalesDepartmentId;

        if (model.SaleManagerId != null)
        {
            if (model.SaleManager is not { IsActive: true })
            {
                throw new EntityNotFoundException("Sale manager not found");
            }

            if (model.SaleManager.DepartmentId != salesDepartmentId
                && !(allowCeoCto && (model.SaleManager.Position.IsCeo() || model.SaleManager.Position.IsCto())))
            {
                throw new InvalidDataAppException(allowCeoCto
                    ? "User is not Sale manager, CEO or CTO"
                    : "User is not Sale manager");
            }

            if (IsSalesEmployeeAssigningInterviewToAnotherSalesEmployee(model, salesDepartmentId))
            {
                throw new InvalidDataAppException(
                    "Sales employees can only assign interviews to themselves and not to other Sale employees");
            }
        }
    }

    private bool IsSalesEmployeeAssigningInterviewToAnotherSalesEmployee(Request? model, string? salesDepartmentId)
    {
        return _authenticatedUserContext.DepartmentId == salesDepartmentId &&
            _authenticatedUserContext.GetRequiredUserId() != model?.SaleManager?.UserId &&
            !_authenticatedUserContext.IsManager;
    }

    public record Request(
        int? SaleManagerId,
        TeamBoardUser? SaleManager);
}

