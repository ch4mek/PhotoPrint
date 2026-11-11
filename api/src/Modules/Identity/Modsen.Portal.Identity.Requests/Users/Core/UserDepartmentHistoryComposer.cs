using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.Requests.Users.Core
{
    internal interface IUserDepartmentHistoryComposer
    {
        void Compose(string? departmentId, User user);
    }

    internal class UserDepartmentHistoryComposer : IUserDepartmentHistoryComposer
    {
        private readonly IClockService _clockService;

        public UserDepartmentHistoryComposer(IClockService clockService)
        {
            _clockService = clockService;
        }

        public async void Compose(string? departmentId, User user)
        {
            if (departmentId != null)
            {
                var currentDepartment = user.UserDepartmentHistory.FirstOrDefault(x => x.DateTo == null);

                if (currentDepartment?.DepartmentId != departmentId)
                {
                    if (currentDepartment != null)
                    {
                        currentDepartment.DateTo = _clockService.TodayUtc().AddDays(-1);
                    }

                    user.UserDepartmentHistory.Add(new UserDepartmentHistory
                    {
                        DepartmentId = departmentId,
                        DateFrom = _clockService.TodayUtc(),
                    });
                }
            }
        }
    }
}
