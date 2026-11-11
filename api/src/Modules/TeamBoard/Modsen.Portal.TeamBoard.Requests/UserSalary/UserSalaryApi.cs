using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.UserSalary.ApiHandlers;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary;

internal interface IUserSalaryApi
{
    Task<bool> UpdateUserSalaryListWithCreating(int userId, IReadOnlyCollection<UserSalaryFullCoreDto> salaries, CancellationToken cancellationToken);
}
internal sealed class UserSalaryApi : IUserSalaryApi
{
    private readonly IUpdateUserSalaryListWithCreatingApiHandler _updateUserSalaryListWithCreatingApi;

    public UserSalaryApi(IUpdateUserSalaryListWithCreatingApiHandler updateUserSalaryListWithCreatingApi)
    {
        _updateUserSalaryListWithCreatingApi = updateUserSalaryListWithCreatingApi;
    }

    public Task<bool> UpdateUserSalaryListWithCreating(int userId, IReadOnlyCollection<UserSalaryFullCoreDto> salaries, CancellationToken cancellationToken)
    {
        return _updateUserSalaryListWithCreatingApi.UpdateAsync(userId, salaries, cancellationToken);
    }
}
