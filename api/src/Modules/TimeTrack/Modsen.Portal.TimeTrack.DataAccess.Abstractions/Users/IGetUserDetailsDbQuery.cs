namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users
{
    internal interface IGetUserDetailsDbQuery
    {
        Task<GetUserDetailsDbQueryResponse?> Execute(int userId, CancellationToken cancellationToken);
    }

    internal class GetUserDetailsDbQueryResponse
    {
        public int UserId { get; set; }
        public string? DepartmentId { get; set; }
        public int? ResourceGroupId { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
    }
}
