using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output
{
    internal class UserInfoShortOutput
    {
        public required int UserId { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public string? FullNameRu { get; set; }
        public string? FirstNameRu { get; set; }
        public string? LastNameRu { get; set; }

        internal static UserInfoShortOutput Of(UserBirthdayInfoDto source)
        {
            return new UserInfoShortOutput
            {
                FullName = source.FullName,
                FirstName = source.FirstName,
                LastName = source.LastName,
                UserId = source.UserId,
                Email = source.Email,
                FirstNameRu = source.FirstNameRu,
                LastNameRu = source.LastNameRu,
                FullNameRu = source.FullNameRu,
            };
        }
    }
}
