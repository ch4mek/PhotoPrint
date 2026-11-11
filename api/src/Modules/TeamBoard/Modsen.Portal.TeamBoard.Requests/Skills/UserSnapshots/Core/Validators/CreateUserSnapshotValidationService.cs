using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.Validators;

internal interface ICreateUserSnapshotValidationService
{
    Task ValidateCommandAsync(CreateUserSnapshotRequest.Command command, CancellationToken cancellationToken);
    Task ValidateHardSkillsListAsync(CreateUserSpecializationDto specialization, CancellationToken cancellationToken);
}

internal sealed class CreateUserSnapshotValidationService(
    ICreateUserSnapshotValidator createUserSnapshotValidator,
    ICreateUserSnapshotHardSkillsListValidator createUserSnapshotHardSkillsListValidator)
    : ICreateUserSnapshotValidationService
{
    public Task ValidateCommandAsync(CreateUserSnapshotRequest.Command command, CancellationToken cancellationToken)
    {
        return createUserSnapshotValidator.ValidateAsync(command, cancellationToken);
    }

    public Task ValidateHardSkillsListAsync(CreateUserSpecializationDto specialization, CancellationToken cancellationToken)
    {
        return createUserSnapshotHardSkillsListValidator.ValidateAsync(specialization, cancellationToken);
    }
}
