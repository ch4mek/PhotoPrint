using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output
{
    internal class UserBirthdayShortOutput
    {
        public required string LastName { get; init; }
        public required string FirstName { get; init; }
        public string? AvatarFileId { get; init; }
        public string? Department { get; set; }

        internal static UserBirthdayShortOutput Of(UserBirthdayInfoDto source)
        {
            return new UserBirthdayShortOutput
            {
                LastName = source.LastName,
                FirstName = source.FirstName,
                AvatarFileId = source.AvatarFileId,
                Department = source.Department
            };
        }
    }
}
