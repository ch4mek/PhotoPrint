namespace Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

internal interface IUserAccessService
{
    Task<bool> CanAccessUserReportInformation(int userId, CancellationToken cancellationToken);
}
