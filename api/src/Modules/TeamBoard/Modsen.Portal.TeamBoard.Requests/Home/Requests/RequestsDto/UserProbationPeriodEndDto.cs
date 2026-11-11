namespace Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto
{
    internal class UserProbationPeriodEndDto
    {
        public required int UserId { get; set; }
        public required string FullName { get; set; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public string? FullNameRu { get; set; }
        public string? FirstNameRu { get; set; }
        public string? LastNameRu { get; set; }
        public required string Email { get; set; }
        public string? DepartmentId { get; set; }
    }
}
